using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.AntiCaptcha.Requests;
using CaptchaSharp.Services.AntiCaptcha.Responses;
using CaptchaSharp.Services.AntiCaptcha.Responses.Solutions;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services
{
    public class AntiCaptchaService : CaptchaService
    {
        public string ApiKey { get; set; }
        protected HttpClient httpClient;

        public int SoftId { get; set; }

        public AntiCaptchaService(string apiKey, HttpClient httpClient = null)
        {
            ApiKey = apiKey;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("https://api.anti-captcha.com");
        }

        #region Getting the Balance
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostJsonAsync
                ("getBalance",
                new Request() { ClientKey = ApiKey },
                cancellationToken)
                .ConfigureAwait(false);

            var balanceResponse = response.Deserialize<GetBalanceResponse>();

            if (balanceResponse.IsError)
                throw new BadAuthenticationException($"{balanceResponse.ErrorCode}: {balanceResponse.ErrorDescription}");

            return new decimal(balanceResponse.Balance);
        }
        #endregion

        #region Solve Methods
        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostJsonAsync
                ("createTask",
                AddCapabilities(new ImageCaptchaRequest()
                    {
                        ClientKey = ApiKey,
                        Type = "ImageToTextTask",
                        Body = base64,
                        SoftId = SoftId
                    }, options),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.ImageCaptcha, cancellationToken)
                as StringResponse;
        }

        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var content = new JObject(CreateCaptchaRequest("NoCaptchaTask", proxy));
            content.Add("websiteURL", siteUrl);
            content.Add("websiteKey", siteKey);
            content.Add("isInvisible", invisible);
            
            var response = await httpClient.PostJsonAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.ReCaptchaV2, cancellationToken)
                as StringResponse;
        }

        public async override Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            if (proxy != null)
                throw new NotSupportedException("Proxies are not supported");

            if (minScore != 0.3F && minScore != 0.7F && minScore != 0.9F)
                throw new NotSupportedException("Only min scores of 0.3, 0.7 and 0.9 are supported");

            var content = new JObject(CreateCaptchaRequest("RecaptchaV3Task", null));
            content.Add("websiteURL", siteUrl);
            content.Add("websiteKey", siteKey);
            content.Add("pageAction", action);
            content.Add("minScore", minScore);

            var response = await httpClient.PostJsonAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.ReCaptchaV3, cancellationToken)
                as StringResponse;
        }

        public async override Task<StringResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var content = new JObject(CreateCaptchaRequest("FunCaptchaTask", null));
            content.Add("websiteURL", siteUrl);
            content.Add("websitePublicKey", publicKey);
            content.Add("funcaptchaApiJSSubdomain", serviceUrl);

            var response = await httpClient.PostJsonAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.FunCaptcha, cancellationToken)
                as StringResponse;
        }

        public async override Task<StringResponse> SolveHCaptchaAsync
            (string siteKey, string siteUrl, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var content = new JObject(CreateCaptchaRequest("HCaptchaTask", null));
            content.Add("websiteURL", siteUrl);
            content.Add("websiteKey", siteKey);

            var response = await httpClient.PostJsonAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.HCaptcha, cancellationToken)
                as StringResponse;
        }

        public async override Task<GeeTestResponse> SolveGeeTestAsync
            (string gt, string challenge, string apiServer, string siteUrl, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var content = new JObject(CreateCaptchaRequest("GeeTestTask", null));
            content.Add("websiteURL", siteUrl);
            content.Add("gt", gt);
            content.Add("challenge", challenge);
            content.Add("geetestApiServerSubdomain", apiServer);

            var response = await httpClient.PostJsonAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.HCaptcha, cancellationToken)
                as GeeTestResponse;
        }
        #endregion

        #region Getting the result
        private async Task<CaptchaResponse> TryGetResult
            (TaskCreationResponse response, CaptchaType type, CancellationToken cancellationToken = default)
        {
            if (response.IsError)
                throw new TaskCreationException($"{response.ErrorCode}: {response.ErrorDescription}");

            var task = new CaptchaTask(response.TaskId, type);

            return await TryGetResult(task, cancellationToken).ConfigureAwait(false);
        }

        internal async override Task<CaptchaResponse> CheckResult(CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostJsonAsync
                ("getTaskResult",
                new GetTaskResultRequest() { ClientKey = ApiKey, TaskId = task.Id },
                cancellationToken).ConfigureAwait(false);

            var result = response.Deserialize<GetTaskResultResponse<object>>();
            ITaskSolution solution;

            switch (task.Type)
            {
                case CaptchaType.ReCaptchaV2:
                case CaptchaType.ReCaptchaV3:
                case CaptchaType.HCaptcha:
                    solution = response
                        .Deserialize<GetTaskResultResponse<RecaptchaSolution>>()
                        .Solution;
                    break;

                case CaptchaType.FunCaptcha:
                    solution = response
                        .Deserialize<GetTaskResultResponse<FuncaptchaSolution>>()
                        .Solution;
                    break;

                case CaptchaType.ImageCaptcha:
                    solution = response
                        .Deserialize<GetTaskResultResponse<ImageCaptchaSolution>>()
                        .Solution;
                    break;

                case CaptchaType.GeeTest:
                    solution = response
                        .Deserialize<GetTaskResultResponse<GeeTestSolution>>()
                        .Solution;
                    break;

                default:
                    throw new NotSupportedException();
            }

            if (!result.IsReady)
                return default;

            task.Completed = true;

            if (result.IsError)
                throw new TaskSolutionException($"{result.ErrorCode}: {result.ErrorDescription}");

            return solution.ToCaptchaResponse(task.Id);
        }
        #endregion

        #region Reporting the solution
        public async override Task ReportSolution
            (int taskId, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            if (correct)
                throw new NotSupportedException("This service doesn't allow reporting of good solutions");

            string response;
            ReportIncorrectCaptchaResponse incResponse;

            switch (type)
            {
                case CaptchaType.ImageCaptcha:
                    response = await httpClient.PostJsonAsync
                    ("reportIncorrectImageCaptcha",
                    new ReportIncorrectCaptchaRequest() { ClientKey = ApiKey, TaskId = taskId },
                    cancellationToken).ConfigureAwait(false);

                    incResponse = response.Deserialize<ReportIncorrectCaptchaResponse>();
                    break;

                case CaptchaType.ReCaptchaV2:
                case CaptchaType.ReCaptchaV3:
                    response = await httpClient.PostJsonAsync
                    ("reportIncorrectImageCaptcha",
                    new ReportIncorrectCaptchaRequest() { ClientKey = ApiKey, TaskId = taskId },
                    cancellationToken).ConfigureAwait(false);

                    incResponse = response.Deserialize<ReportIncorrectCaptchaResponse>();
                    break;

                default:
                    throw new NotSupportedException("Reporting is not supported for this captcha type");
            }

            if (incResponse.NotFoundOrExpired)
                throw new TaskReportException("Captcha not found or expired");
        }
        #endregion

        #region Private Methods
        private CaptchaTaskRequest CreateCaptchaRequest(string type, Proxy proxy = null)
        {
            if (proxy != null)
            {
                return new CaptchaTaskProxyRequest()
                {
                    ClientKey = ApiKey,
                    Type = type
                }
                .SetProxy(proxy);
            }
            else
            {
                return new CaptchaTaskRequest()
                {
                    ClientKey = ApiKey,
                    Type = type + "Proxyless"
                };
            }
        }
        #endregion

        #region Capabilities
        public override CaptchaServiceCapabilities Capabilities =>
            CaptchaServiceCapabilities.Language |
            CaptchaServiceCapabilities.Phrases |
            CaptchaServiceCapabilities.CaseSensitivity |
            CaptchaServiceCapabilities.CharacterSets |
            CaptchaServiceCapabilities.Calculations |
            CaptchaServiceCapabilities.MinLength |
            CaptchaServiceCapabilities.MaxLength |
            CaptchaServiceCapabilities.Instructions;

        private ImageCaptchaRequest AddCapabilities(ImageCaptchaRequest request, ImageCaptchaOptions options)
        {
            request.Phrase = options.IsPhrase;
            request.Case = options.CaseSensitive;
            
            switch (options.CharacterSet)
            {
                case CharacterSet.OnlyNumbers:
                    request.Numeric = 1;
                    break;

                case CharacterSet.OnlyLetters:
                    request.Numeric = 2;
                    break;

                default:
                    request.Numeric = 0;
                    break;
            }

            request.Math = options.RequiresCalculation;
            request.MinLength = options.MinLength;
            request.MaxLength = options.MaxLength;
            request.Comment = options.TextInstructions;
            
            switch (options.CaptchaLanguage)
            {
                case CaptchaLanguage.English:
                    request.LanguagePool = "en";
                    break;

                case CaptchaLanguage.Russian:
                case CaptchaLanguage.Ukrainian:
                case CaptchaLanguage.Kazakh:
                case CaptchaLanguage.Belorussian:
                    request.LanguagePool = "rn";
                    break;

                default:
                    throw new NotSupportedException($"The {options.CaptchaLanguage} language is not supported");
            }

            return request;
        }
        #endregion
    }
}
