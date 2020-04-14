using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services
{
    public class ImageTyperzService : CaptchaService
    {
        public string ApiKey { get; set; }
        protected HttpClient httpClient;
        public string AffiliateId { get; set; } = "123";

        public ImageTyperzService(string apiKey, HttpClient httpClient = null)
        {
            ApiKey = apiKey;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("http://captchatypers.com");

            // Since this service replies directly with the solution to the task creation request (for image captchas)
            // we need to set a high timeout here or it will not finish in time
            this.httpClient.Timeout = Timeout;
        }

        #region Getting the Balance
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostToStringAsync
                ("Forms/RequestBalanceToken.ashx",
                GetAuthPair()
                .Add("action", "REQUESTBALANCE"),
                cancellationToken)
                .ConfigureAwait(false);

            if (IsError(response))
                throw new BadAuthenticationException(GetErrorMessage(response));

            return decimal.Parse(response, CultureInfo.InvariantCulture);
        }
        #endregion

        #region Solve Methods
        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostToStringAsync
                ("Forms/UploadFileAndGetTextNEWToken.ashx",
                GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("file", base64)
                .Add(ConvertCapabilities(options)),
                cancellationToken)
                .ConfigureAwait(false);

            if (IsError(response))
                throw new TaskSolutionException(GetErrorMessage(response));

            var split = response.Split(new char[] { '|' }, 2);
            return new StringResponse { Id = long.Parse(split[0]), Response = split[1] };
        }

        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostToStringAsync
                ("captchaapi/UploadRecaptchaToken.ashx",
                GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("pageurl", siteUrl)
                .Add("googlekey", siteKey)
                .Add("recaptchatype", invisible ? 2 : 1)
                .Add(GetProxyParams(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.ReCaptchaV2, cancellationToken) as StringResponse;
        }

        public async override Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action, float minScore, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostToStringAsync
                ("captchaapi/UploadRecaptchaToken.ashx",
                GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("pageurl", siteUrl)
                .Add("googlekey", siteKey)
                .Add("action", action)
                .Add("score", minScore.ToString("0.0", CultureInfo.InvariantCulture))
                .Add("recaptchatype", 3)
                .Add(GetProxyParams(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.ReCaptchaV2, cancellationToken) as StringResponse;
        }

        public async override Task<StringResponse> SolveHCaptchaAsync
            (string siteKey, string siteUrl, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostToStringAsync
                ("captchaapi/UploadRecaptchaToken.ashx",
                GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("pageurl", $"{siteUrl}--hcaptcha")
                .Add("googlekey", $"{siteKey}--hcaptcha")
                .Add(GetProxyParams(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.HCaptcha, cancellationToken) as StringResponse;
        }

        public async override Task<GeeTestResponse> SolveGeeTestAsync
            (string gt, string challenge, string apiServer, string siteUrl, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("captchaapi/UploadGeeTestToken.ashx?",
                GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("gt", gt)
                .Add("challenge", challenge)
                .Add("domain", siteUrl),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.GeeTest, cancellationToken) as GeeTestResponse;
        }

        public async override Task<StringResponse> SolveCapyAsync
            (string siteKey, string siteUrl, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostToStringAsync
                ("captchaapi/UploadRecaptchaToken.ashx",
                GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("pageurl", $"{siteUrl}--capy")
                .Add("googlekey", $"{siteKey}--capy")
                .Add(GetProxyParams(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.Capy, cancellationToken) as StringResponse;
        }
        #endregion

        #region Getting the result
        private async Task<CaptchaResponse> TryGetResult
            (string response, CaptchaType type, CancellationToken cancellationToken = default)
        {
            if (IsError(response))
                throw new TaskCreationException(response);

            var task = new CaptchaTask(response, type);

            return await TryGetResult(task, cancellationToken).ConfigureAwait(false);
        }

        internal async override Task<CaptchaResponse> CheckResult
            (CaptchaTask task, CancellationToken cancellationToken = default)
        {
            string response;

            if (task.Type == CaptchaType.GeeTest)
            {
                response = await httpClient.GetStringAsync
                    ("captchaapi/getrecaptchatext.ashx?",
                    GetAuthPair()
                    .Add("action", "GETTEXT")
                    .Add("captchaID", task.Id),
                    cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                response = await httpClient.PostToStringAsync
                    ("captchaapi/GetRecaptchaTextToken.ashx",
                    GetAuthPair()
                    .Add("action", "GETTEXT")
                    .Add("captchaID", task.Id),
                    cancellationToken)
                    .ConfigureAwait(false);
            }

            if (response.Contains("NOT_DECODED"))
                return default;

            task.Completed = true;

            if (IsError(response))
                throw new TaskSolutionException(response);

            if (task.Type == CaptchaType.GeeTest) 
            {
                var split = response.Split(new string[] { ";;;" }, 3, StringSplitOptions.None);
                return new GeeTestResponse
                {
                    Challenge = split[0],
                    Validate = split[1],
                    SecCode = split[2]
                };
            }
            else
            {
                return new StringResponse { Id = task.Id, Response = response };
            }
        }
        #endregion

        #region Reporting the solution
        public async override Task ReportSolution
            (long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostToStringAsync
                ("Forms/SetBadImageToken.ashx",
                GetAuthPair()
                .Add("imageid", id)
                .Add("action", "SETBADIMAGE"),
                cancellationToken)
                .ConfigureAwait(false);

            if (response != "SUCCESS")
                throw new TaskReportException(response);
        }
        #endregion

        #region Private Methods
        private StringPairCollection GetAuthPair() 
            => new StringPairCollection().Add("token", ApiKey);

        private StringPairCollection GetAuthAffiliatePair()
            => GetAuthPair().Add("affiliateid", AffiliateId);

        private bool IsError(string response) 
            => response.StartsWith("ERROR:");

        private string GetErrorMessage(string response)
            => response.Replace("ERROR: ", "");

        private IEnumerable<(string, string)> GetProxyParams(Proxy proxy)
        {
            if (proxy == null)
                return new (string, string)[] { };

            if (proxy.Type != ProxyType.HTTP && proxy.Type != ProxyType.HTTPS)
                throw new NotSupportedException("The api only supports HTTP proxies");

            var proxyPairs = new List<(string, string)>
            {
                ("useragent", proxy.UserAgent),
                ("proxytype", "HTTP")
            };

            if (proxy.RequiresAuthentication)
                proxyPairs.Add(("proxy", $"{proxy.Host}:{proxy.Port}:{proxy.Username}:{proxy.Password}"));
            else
                proxyPairs.Add(("proxy", $"{proxy.Host}:{proxy.Port}"));

            return proxyPairs;
        }
        #endregion

        #region Capabilities
        private IEnumerable<(string, string)> ConvertCapabilities(ImageCaptchaOptions options)
        {
            // If null, don't return any parameters
            if (options == null)
                return new (string, string)[] { };

            var capabilities = new List<(string, string)> 
            { 
                ("iscase", options.CaseSensitive.ToString().ToLower()),
                ("isphrase", options.IsPhrase.ToString().ToLower()),
                ("ismath", options.RequiresCalculation.ToString().ToLower()),
                ("minlength", options.MinLength.ToString()),
                ("maxlength", options.MaxLength.ToString())
            };

            int alphanumeric;
            switch (options.CharacterSet)
            {
                default:
                    alphanumeric = 0;
                    break;

                case CharacterSet.OnlyNumbers:
                    alphanumeric = 1;
                    break;

                case CharacterSet.OnlyLetters:
                    alphanumeric = 2;
                    break;
            }

            capabilities.Add(("alphanumeric", alphanumeric.ToString()));

            return capabilities;
        }
        #endregion
    }
}
