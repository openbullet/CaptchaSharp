using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.CapSolver.Requests;
using CaptchaSharp.Services.CapSolver.Requests.Tasks;
using CaptchaSharp.Services.CapSolver.Requests.Tasks.Proxied;
using CaptchaSharp.Services.CapSolver.Responses;
using CaptchaSharp.Services.CapSolver.Responses.Solutions;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services
{
    /// <summary>The service provided by <c>https://www.CapSolver.io/</c></summary>
    public class CapSolverService : CaptchaService
    {
        /// <summary>Your secret api key.</summary>
        public string ApiKey { get; set; }

        /// <summary>The default <see cref="HttpClient"/> used for requests.</summary>
        protected HttpClient httpClient;

        /// <summary>The ID of the app.</summary>
        private readonly string appId = "FE552FC0-8A06-4B44-BD30-5B9DDA2A4194";

        /// <summary>Initializes a <see cref="CapSolverService"/> using the given <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public CapSolverService(string apiKey, HttpClient httpClient = null)
        {
            ApiKey = apiKey;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("https://api.capsolver.com");
        }

        #region Getting the Balance
        /// <inheritdoc/>
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostJsonToStringAsync
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
        /// <inheritdoc/>
        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostJsonToStringAsync
                ("createTask",
                new CaptchaTaskRequest
                {
                    ClientKey = ApiKey,
                    AppId = appId,
                    Task = new ImageCaptchaTask
                    {
                        Body = base64
                    }
                },
                cancellationToken)
                .ConfigureAwait(false);

            // The image captcha task immediately returns the solution
            var result = response.Deserialize<GetTaskResultResponse>();
            
            if (result.IsError)
                throw new TaskSolutionException($"{result.ErrorCode}: {result.ErrorDescription}");

            var jObject = JObject.Parse(response);
            var solution = jObject["solution"];
            var taskId = jObject["taskId"].Value<string>();

            result.Solution = solution.ToObject<ImageCaptchaSolution>();

            return result.Solution.ToCaptchaResponse(taskId) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var content = CreateTaskRequest();

            if (enterprise)
            {
                if (proxy != null)
                {
                    content.Task = new RecaptchaV2EnterpriseTask
                    {
                        WebsiteKey = siteKey,
                        WebsiteURL = siteUrl,
                        EnterprisePayload = dataS
                    }.SetProxy(proxy);
                }
                else
                {
                    content.Task = new RecaptchaV2EnterpriseTaskProxyless
                    {
                        WebsiteKey = siteKey,
                        WebsiteURL = siteUrl,
                        EnterprisePayload = dataS
                    };
                }
            }
            else
            {
                if (proxy != null)
                {
                    content.Task = new RecaptchaV2Task
                    {
                        WebsiteKey = siteKey,
                        WebsiteURL = siteUrl,
                        IsInvisible = invisible,
                        RecaptchaDataSValue = dataS
                    }.SetProxy(proxy);
                }
                else
                {
                    content.Task = new RecaptchaV2TaskProxyless
                    {
                        WebsiteKey = siteKey,
                        WebsiteURL = siteUrl,
                        IsInvisible = invisible,
                        RecaptchaDataSValue = dataS
                    };
                }
            }

            var response = await httpClient.PostJsonToStringAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.ReCaptchaV2, cancellationToken)
                as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, bool enterprise = false,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var content = CreateTaskRequest();

            if (proxy is null)
            {
                content.Task = new RecaptchaV3TaskProxyless
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    PageAction = action,
                    MinScore = minScore,
                    IsEnterprise = enterprise
                };
            }
            else
            {
                content.Task = new RecaptchaV3Task
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    PageAction = action,
                    MinScore = minScore,
                    IsEnterprise = enterprise
                }.SetProxy(proxy);
            }

            var response = await httpClient.PostJsonToStringAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.ReCaptchaV3, cancellationToken)
                as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            if (noJS)
                throw new NotSupportedException("This service does not support no js solving");

            var content = CreateTaskRequest();

            if (proxy != null)
            {
                content.Task = new FunCaptchaTask
                {
                    WebsitePublicKey = publicKey,
                    WebsiteURL = siteUrl,
                    FuncaptchaApiJSSubdomain = serviceUrl
                }.SetProxy(proxy);
            }
            else
            {
                content.Task = new FunCaptchaTaskProxyless
                {
                    WebsitePublicKey = publicKey,
                    WebsiteURL = siteUrl,
                    FuncaptchaApiJSSubdomain = serviceUrl
                };
            }

            var response = await httpClient.PostJsonToStringAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.FunCaptcha, cancellationToken)
                as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveHCaptchaAsync
            (string siteKey, string siteUrl, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var content = CreateTaskRequest();

            if (proxy != null)
            {
                content.Task = new HCaptchaTask
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                }.SetProxy(proxy);
            }
            else
            {
                content.Task = new HCaptchaTaskProxyless
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                };
            }

            var response = await httpClient.PostJsonToStringAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.HCaptcha, cancellationToken)
                as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<GeeTestResponse> SolveGeeTestAsync
            (string gt, string challenge, string apiServer, string siteUrl, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var content = CreateTaskRequest();

            if (proxy != null)
            {
                content.Task = new GeeTestTask
                {
                    WebsiteURL = siteUrl,
                    Gt = gt,
                    Challenge = challenge,
                    GeetestApiServerSubdomain = apiServer
                }.SetProxy(proxy);
            }
            else
            {
                content.Task = new GeeTestTaskProxyless
                {
                    WebsiteURL = siteUrl,
                    Gt = gt,
                    Challenge = challenge,
                    GeetestApiServerSubdomain = apiServer,
                    Version = 3
                };
            }

            var response = await httpClient.PostJsonToStringAsync
                ("createTask",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response.Deserialize<TaskCreationResponse>(), CaptchaType.GeeTest, cancellationToken)
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

        /// <inheritdoc/>
        protected async override Task<CaptchaResponse> CheckResult
            (CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostJsonToStringAsync
                ("getTaskResult",
                new GetTaskResultRequest() { ClientKey = ApiKey, TaskId = task.IdString },
                cancellationToken).ConfigureAwait(false);

            var result = response.Deserialize<GetTaskResultResponse>();

            if (!result.IsReady)
                return default;

            task.Completed = true;

            if (result.IsError)
                throw new TaskSolutionException($"{result.ErrorCode}: {result.ErrorDescription}");

            var jObject = JObject.Parse(response);
            var solution = jObject["solution"];

            switch (task.Type)
            {
                case CaptchaType.ReCaptchaV2:
                case CaptchaType.ReCaptchaV3:
                case CaptchaType.HCaptcha:
                    result.Solution = solution.ToObject<RecaptchaSolution>();
                    break;

                case CaptchaType.FunCaptcha:
                    result.Solution = solution.ToObject<FuncaptchaSolution>();
                    break;

                case CaptchaType.ImageCaptcha:
                    result.Solution = solution.ToObject<ImageCaptchaSolution>();
                    break;

                case CaptchaType.GeeTest:
                    result.Solution = solution.ToObject<GeeTestSolution>();
                    break;

                default:
                    throw new NotSupportedException();
            }

            return result.Solution.ToCaptchaResponse(task.IdString);
        }
        #endregion

        #region Private Methods
        private CaptchaTaskRequest CreateTaskRequest()
        {
            return new CaptchaTaskRequest
            {
                ClientKey = ApiKey,
                AppId = appId
            };
        }
        #endregion

        #region Capabilities
        /// <inheritdoc/>
        public override CaptchaServiceCapabilities Capabilities =>
            CaptchaServiceCapabilities.None;
        #endregion
    }
}
