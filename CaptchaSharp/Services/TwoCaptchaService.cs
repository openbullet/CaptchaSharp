using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.TwoCaptcha;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services
{
    /// <summary>The service provided by <c>https://2captcha.com/</c></summary>
    public class TwoCaptchaService : CaptchaService
    {
        /// <summary>Your secret api key.</summary>
        public string ApiKey { get; set; }

        /// <summary>The default <see cref="HttpClient"/> used for requests.</summary>
        protected HttpClient httpClient;

        /// <summary>Set it to false if the service does not support json responses.</summary>
        public bool UseJsonFlag { get; set; } = true;

        /// <summary>Will include an Access-Control-Allow-Origin:* header in the response for 
        /// cross-domain AJAX requests in web applications.</summary>
        public bool AddACAOHeader { get; set; } = false;

        /// <summary>The ID of the software developer.</summary>
        public int SoftId { get; set; } = 2658;

        /// <summary>Initializes a <see cref="TwoCaptchaService"/> using the given <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public TwoCaptchaService(string apiKey, HttpClient httpClient = null)
        {
            ApiKey = apiKey;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("http://2captcha.com");
        }

        #region Getting the Balance
        /// <inheritdoc/>
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("res.php",
                new StringPairCollection() 
                    .Add("key", ApiKey)
                    .Add("action", "getbalance")
                    .Add("json", Convert.ToInt32(UseJsonFlag).ToString()),
                cancellationToken);

            if (UseJsonFlag)
            {
                var tcResponse = response.Deserialize<Response>();

                if (tcResponse.IsErrorCode)
                    throw new BadAuthenticationException(tcResponse.Request);

                return decimal.Parse(tcResponse.Request, CultureInfo.InvariantCulture);
            }
            else
            {
                if (decimal.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal balance))
                    return balance;

                else
                    throw new BadAuthenticationException(response);
            }
        }
        #endregion

        #region Solve Methods
        /// <inheritdoc/>
        public async override Task<StringResponse> SolveTextCaptchaAsync
            (string text, TextCaptchaOptions options = default, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("textcaptcha", text)
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertCapabilities(options))
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.TextCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.TextCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("method", "base64")
                    .Add("body", base64)
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertCapabilities(options))
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("method", "userrecaptcha")
                    .Add("googlekey", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add("invisible", Convert.ToInt32(invisible).ToString())
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy))
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("method", "userrecaptcha")
                    .Add("version", "v3")
                    .Add("googlekey", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add("action", action)
                    .Add("min_score", minScore.ToString("0.0", CultureInfo.InvariantCulture))
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy))
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.ReCaptchaV3, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.ReCaptchaV3, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("method", "funcaptcha")
                    .Add("publickey", publicKey)
                    .Add("surl", serviceUrl)
                    .Add("pageurl", siteUrl)
                    .Add("nojs", Convert.ToInt32(noJS).ToString())
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy))
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.FunCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.FunCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveHCaptchaAsync
            (string siteKey, string siteUrl, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("method", "hcaptcha")
                    .Add("sitekey", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy))
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.HCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.HCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveKeyCaptchaAsync
            (string userId, string sessionId, string webServerSign1, string webServerSign2, string siteUrl,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("method", "keycaptcha")
                    .Add("s_s_c_user_id", userId)
                    .Add("s_s_c_session_id", sessionId)
                    .Add("s_s_c_web_server_sign", webServerSign1)
                    .Add("s_s_c_web_server_sign2", webServerSign2)
                    .Add("pageurl", siteUrl)
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy))
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.KeyCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.KeyCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<GeeTestResponse> SolveGeeTestAsync
            (string gt, string challenge, string apiServer, string siteUrl, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("method", "geetest")
                    .Add("gt", gt)
                    .Add("challenge", challenge)
                    .Add("api_server", apiServer)
                    .Add("pageurl", siteUrl)
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy))
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.GeeTest, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.GeeTest, cancellationToken).ConfigureAwait(false)
                ) as GeeTestResponse;
        }
        #endregion

        #region Getting the result
        internal async Task<CaptchaResponse> TryGetResult
            (Response response, CaptchaType type, CancellationToken cancellationToken = default)
        {
            if (response.IsErrorCode)
                throw new TaskCreationException(response.Request);

            var task = new CaptchaTask(response.Request, type);

            return await TryGetResult(task, cancellationToken).ConfigureAwait(false);
        }

        internal async Task<CaptchaResponse> TryGetResult
            (string response, CaptchaType type, CancellationToken cancellationToken = default)
        {
            if (IsErrorCode(response))
                throw new TaskCreationException(response);

            var task = new CaptchaTask(TakeSecondSlice(response), type);

            return await TryGetResult(task, cancellationToken).ConfigureAwait(false);
        }

        /// <summary></summary>
        protected async override Task<CaptchaResponse> CheckResult
            (CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("res.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("action", "get")
                    .Add("id", task.Id.ToString())
                    .Add("json", Convert.ToInt32(UseJsonFlag).ToString()),
                cancellationToken);

            if (response.Contains("CAPCHA_NOT_READY"))
                return default;

            task.Completed = true;

            if (UseJsonFlag)
            {
                if (task.Type == CaptchaType.GeeTest)
                {
                    var jObject = JObject.Parse(response);
                    var solution = jObject["request"];

                    if (solution.Type == JTokenType.Object)
                    {
                        return response.Deserialize<TwoCaptchaGeeTestResponse>()
                            .Request.ToGeeTestResponse(task.Id);
                    }
                }

                var tcResponse = response.Deserialize<Response>();

                if (tcResponse.IsErrorCode)
                    throw new TaskSolutionException(tcResponse.Error_Text);

                return new StringResponse { Id = task.Id, Response = tcResponse.Request };
            }
            else
            {
                if (IsErrorCode(response))
                    throw new TaskSolutionException(response);

                response = TakeSecondSlice(response);

                switch (task.Type)
                {
                    case CaptchaType.GeeTest:
                        return response.Deserialize<GeeTestSolution>().ToGeeTestResponse(task.Id);

                    default:
                        return new StringResponse { Id = task.Id, Response = response };
                }
            }
        }
        #endregion

        #region Reporting the solution
        /// <inheritdoc/>
        public async override Task ReportSolution
            (long taskId, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            var action = correct ? "reportgood" : "reportbad";

            var response = await httpClient.GetStringAsync
                ("res.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("action", action)
                    .Add("id", taskId.ToString())
                    .Add("json", Convert.ToInt32(UseJsonFlag).ToString()),
                cancellationToken);

            if (UseJsonFlag)
            {
                var tcResponse = response.Deserialize<Response>();

                if (tcResponse.IsErrorCode)
                    throw new TaskReportException(tcResponse.Request);
            }
            else
            {
                if (IsErrorCode(response))
                    throw new TaskReportException(response);
            }
        }
        #endregion

        #region Proxies
        /// <summary></summary>
        protected IEnumerable<(string, string)> ConvertProxy(Proxy proxy)
        {
            if (proxy == null)
                return new (string, string)[] { };

            return new (string, string)[]
            {
                ("proxy", proxy.RequiresAuthentication 
                    ? $"{proxy.Username}:{proxy.Password}@{proxy.Host}:{proxy.Port}"
                    : $"{proxy.Host}:{proxy.Port}"),
                ("proxytype", proxy.Type.ToString())
            };
        }
        #endregion

        #region Utility methods
        /// <summary>For non-json response.</summary>
        protected bool IsErrorCode(string response)
        {
            return !response.StartsWith("OK");
        }

        /// <summary>For non-json response.</summary>
        protected string TakeSecondSlice(string str)
        {
            return str.Split('|')[1];
        }
        #endregion

        #region Capabilities
        /// <inheritdoc/>
        public new CaptchaServiceCapabilities Capabilities =>
            CaptchaServiceCapabilities.LanguageGroup |
            CaptchaServiceCapabilities.Language |
            CaptchaServiceCapabilities.Phrases |
            CaptchaServiceCapabilities.CaseSensitivity |
            CaptchaServiceCapabilities.CharacterSets |
            CaptchaServiceCapabilities.Calculations |
            CaptchaServiceCapabilities.MinLength |
            CaptchaServiceCapabilities.MaxLength |
            CaptchaServiceCapabilities.Instructions;

        /// <summary></summary>
        protected IEnumerable<(string, string)> ConvertCapabilities(TextCaptchaOptions options)
        {
            // If null, don't return any parameters
            if (options == null)
                return new (string, string)[] { };

            var capabilities = new List<(string, string)>();

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.LanguageGroup))
                capabilities.Add(("language", ((int)options.CaptchaLanguageGroup).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Language) && options.CaptchaLanguage != CaptchaLanguage.NotSpecified)
                capabilities.Add(("lang", options.CaptchaLanguage.ToISO6391Code()));

            return capabilities;
        }

        /// <summary></summary>
        protected IEnumerable<(string, string)> ConvertCapabilities(ImageCaptchaOptions options)
        {
            // If null, don't return any parameters
            if (options == null)
                return new (string, string)[] { };

            var capabilities = new List<(string, string)>();

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Phrases))
                capabilities.Add(("phrase", Convert.ToInt32(options.IsPhrase).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.CaseSensitivity))
                capabilities.Add(("regsense", Convert.ToInt32(options.CaseSensitive).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.CharacterSets))
                capabilities.Add(("numeric", ((int)options.CharacterSet).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Calculations))
                capabilities.Add(("calc", Convert.ToInt32(options.RequiresCalculation).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.MinLength))
                capabilities.Add(("min_len", options.MinLength.ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.MaxLength))
                capabilities.Add(("max_len", options.MaxLength.ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Instructions))
                capabilities.Add(("textinstructions", options.TextInstructions));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.LanguageGroup))
                capabilities.Add(("language", ((int)options.CaptchaLanguageGroup).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Language) && options.CaptchaLanguage != CaptchaLanguage.NotSpecified)
                capabilities.Add(("lang", options.CaptchaLanguage.ToISO6391Code()));

            return capabilities;
        }
        #endregion
    }
}
