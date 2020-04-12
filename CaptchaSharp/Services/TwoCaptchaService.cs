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

namespace CaptchaSharp.Services
{
    public class TwoCaptchaService : CaptchaService
    {
        public string ApiKey { get; set; }
        protected HttpClient httpClient;
        
        public bool UseJsonFlag { get; set; } = true;
        public bool AddACAOHeader { get; set; } = false;
        public string SoftId { get; set; } = "PLACEHOLDER";

        public TwoCaptchaService(string apiKey, HttpClient httpClient = null)
        {
            ApiKey = apiKey;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("http://2captcha.com");
        }

        #region Getting the Balance
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ($"res.php?key={ApiKey}&action=getbalance&json=" + UseJsonFlag.ToInt().ToString(), cancellationToken);

            if (UseJsonFlag)
            {
                var tcResponse = response.Deserialize<Response>();

                if (tcResponse.IsErrorCode)
                    throw new BadAuthenticationException(tcResponse.Request);

                return decimal.Parse(tcResponse.Request, CultureInfo.InvariantCulture);
            }
            else
            {
                if (IsErrorCode(response))
                    throw new BadAuthenticationException(response);

                return decimal.Parse(response, CultureInfo.InvariantCulture);
            }
        }
        #endregion

        #region Solve Methods
        public async override Task<StringResponse> SolveTextCaptchaAsync
            (string text, TextCaptchaOptions options = default, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new MultipartFormDataContent()
                    .Add("key", ApiKey)
                    .Add("textcaptcha", text)
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertCapabilities(options)),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.TextCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.TextCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new MultipartFormDataContent()
                    .Add("key", ApiKey)
                    .Add("method", "base64")
                    .Add("body", base64)
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertCapabilities(options)),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new MultipartFormDataContent()
                    .Add("key", ApiKey)
                    .Add("method", "userrecaptcha")
                    .Add("googlekey", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add("invisible", invisible.ToInt().ToString())
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        public async override Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new MultipartFormDataContent()
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
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.ReCaptchaV3, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.ReCaptchaV3, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        public async override Task<StringResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new MultipartFormDataContent()
                    .Add("key", ApiKey)
                    .Add("method", "funcaptcha")
                    .Add("publickey", publicKey)
                    .Add("surl", serviceUrl)
                    .Add("pageurl", siteUrl)
                    .Add("nojs", noJS.ToInt().ToString())
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.FunCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.FunCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        public async override Task<StringResponse> SolveHCaptchaAsync
            (string siteKey, string siteUrl, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new MultipartFormDataContent()
                    .Add("key", ApiKey)
                    .Add("method", "hcaptcha")
                    .Add("sitekey", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.HCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.HCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        public async override Task<StringResponse> SolveKeyCaptchaAsync
            (string userId, string sessionId, string webServerSign1, string webServerSign2, string siteUrl,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new MultipartFormDataContent()
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
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.KeyCaptcha, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.KeyCaptcha, cancellationToken).ConfigureAwait(false)
                ) as StringResponse;
        }

        public async override Task<GeeTestResponse> SolveGeeTestAsync
            (string gt, string challenge, string apiServer, string siteUrl, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new MultipartFormDataContent()
                    .Add("key", ApiKey)
                    .Add("method", "geetest")
                    .Add("gt", gt)
                    .Add("challenge", challenge)
                    .Add("api_server", apiServer)
                    .Add("pageurl", siteUrl)
                    .Add("soft_id", SoftId)
                    .Add("json", "1", UseJsonFlag)
                    .Add("header_acao", "1", AddACAOHeader)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return (UseJsonFlag
                ? await TryGetResult(response.Deserialize<Response>(), CaptchaType.GeeTest, cancellationToken).ConfigureAwait(false)
                : await TryGetResult(response, CaptchaType.GeeTest, cancellationToken).ConfigureAwait(false)
                ) as GeeTestResponse;
        }
        #endregion

        #region Getting the result
        private async Task<CaptchaResponse> TryGetResult
            (Response response, CaptchaType type, CancellationToken cancellationToken = default)
        {
            if (response.IsErrorCode)
                throw new TaskCreationException(response.Request);

            var task = new CaptchaTask(response.Request, type);

            return await TryGetResult(task, cancellationToken).ConfigureAwait(false);
        }

        private async Task<CaptchaResponse> TryGetResult
            (string response, CaptchaType type, CancellationToken cancellationToken = default)
        {
            if (IsErrorCode(response))
                throw new TaskCreationException(response);

            var task = new CaptchaTask(TakeSecondSlice(response), type);

            return await TryGetResult(task, cancellationToken).ConfigureAwait(false);
        }

        internal async override Task<CaptchaResponse> CheckResult
            (CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ($"res.php?key={ApiKey}&action=get&id={task.Id}&json=" + UseJsonFlag.ToInt().ToString(), cancellationToken).ConfigureAwait(false);

            if (response.Contains("CAPCHA_NOT_READY"))
                return default;

            task.Completed = true;

            if (UseJsonFlag)
            {
                var tcResponse = response.Deserialize<Response>();
                
                if (tcResponse.IsErrorCode)
                    throw new TaskSolutionException(tcResponse.Request);

                switch (task.Type)
                {
                    case CaptchaType.GeeTest:
                        return tcResponse.Request.Deserialize<GeeTestSolution>().ToGeeTestResponse(task.Id);

                    default:
                        return new StringResponse { Id = task.Id, Response = tcResponse.Request };
                }
            }
            else
            {
                if (IsErrorCode(response))
                    throw new TaskSolutionException(response);

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
        public async override Task ReportSolution
            (int taskId, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            var action = correct ? "reportgood" : "reportbad";

            var response = await httpClient.GetStringAsync
                ($"res.php?key={ApiKey}&action={action}&id={taskId}&json=" + UseJsonFlag.ToInt().ToString(), cancellationToken).ConfigureAwait(false);

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
        private IEnumerable<(string, string)> ConvertProxy(Proxy proxy)
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
        // For non-json response
        private bool IsErrorCode(string response)
        {
            return !response.StartsWith("OK");
        }

        private string TakeSecondSlice(string str)
        {
            return str.Split('|')[1];
        }
        #endregion

        #region Capabilities
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

        private IEnumerable<(string, string)> ConvertCapabilities(TextCaptchaOptions options)
        {
            // If null, don't return any parameters
            if (options == null)
                return new (string, string)[] { };

            var capabilities = new List<(string, string)>();

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.LanguageGroup))
                capabilities.Add(("language", ((int)options.CaptchaLanguageGroup).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Language) && options.CaptchaLanguage != CaptchaLanguage.NotSpecified)
                capabilities.Add(("lang", GetLanguageCode(options.CaptchaLanguage)));

            return capabilities;
        }

        private IEnumerable<(string, string)> ConvertCapabilities(ImageCaptchaOptions options)
        {
            // If null, don't return any parameters
            if (options == null)
                return new (string, string)[] { };

            var capabilities = new List<(string, string)>();

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Phrases))
                capabilities.Add(("phrase", options.IsPhrase.ToInt().ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.CaseSensitivity))
                capabilities.Add(("regsense", options.CaseSensitive.ToInt().ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.CharacterSets))
                capabilities.Add(("numeric", ((int)options.CharacterSet).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Calculations))
                capabilities.Add(("calc", options.RequiresCalculation.ToInt().ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.MinLength))
                capabilities.Add(("min_len", options.MinLength.ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.MaxLength))
                capabilities.Add(("max_len", options.MaxLength.ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Instructions))
                capabilities.Add(("textinstructions", options.TextInstructions));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.LanguageGroup))
                capabilities.Add(("language", ((int)options.CaptchaLanguageGroup).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Language) && options.CaptchaLanguage != CaptchaLanguage.NotSpecified)
                capabilities.Add(("lang", GetLanguageCode(options.CaptchaLanguage)));

            return capabilities;
        }
        
        private string GetLanguageCode(CaptchaLanguage language)
        {
            var dict = new Dictionary<CaptchaLanguage, string>
            {
                { CaptchaLanguage.NotSpecified, "en" },
                { CaptchaLanguage.English,      "en" },
                { CaptchaLanguage.Russian,      "ru" },
                { CaptchaLanguage.Spanish,      "es" },
                { CaptchaLanguage.Portuguese,   "pt" },
                { CaptchaLanguage.Ukrainian,    "uk" },
                { CaptchaLanguage.Vietnamese,   "vi" },
                { CaptchaLanguage.French,       "fr" },
                { CaptchaLanguage.Indonesian,   "id" },
                { CaptchaLanguage.Arab,         "ar" },
                { CaptchaLanguage.Japanese,     "ja" },
                { CaptchaLanguage.Turkish,      "tr" },
                { CaptchaLanguage.German,       "de" },
                { CaptchaLanguage.Chinese,      "zh" },
                { CaptchaLanguage.Philippine,   "fil" },
                { CaptchaLanguage.Polish,       "pl" },
                { CaptchaLanguage.Thai,         "th" },
                { CaptchaLanguage.Italian,      "it" },
                { CaptchaLanguage.Dutch,        "nl" },
                { CaptchaLanguage.Slovak,       "sk" },
                { CaptchaLanguage.Bulgarian,    "bg" },
                { CaptchaLanguage.Romanian,     "ro" },
                { CaptchaLanguage.Hungarian,    "hu" },
                { CaptchaLanguage.Korean,       "ko" },
                { CaptchaLanguage.Czech,        "cs" },
                { CaptchaLanguage.Azerbaijani,  "az" },
                { CaptchaLanguage.Persian,      "fa" },
                { CaptchaLanguage.Bengali,      "bn" },
                { CaptchaLanguage.Greek,        "el" },
                { CaptchaLanguage.Lithuanian,   "lt" },
                { CaptchaLanguage.Latvian,      "lv" },
                { CaptchaLanguage.Swedish,      "sv" },
                { CaptchaLanguage.Serbian,      "sr" },
                { CaptchaLanguage.Croatian,     "hr" },
                { CaptchaLanguage.Hebrew,       "he" },
                { CaptchaLanguage.Hindi,        "hi" },
                { CaptchaLanguage.Norwegian,    "nb" },
                { CaptchaLanguage.Slovenian,    "sl" },
                { CaptchaLanguage.Danish,       "da" },
                { CaptchaLanguage.Uzbek,        "uz" },
                { CaptchaLanguage.Finnish,      "fi" },
                { CaptchaLanguage.Catalan,      "ca" },
                { CaptchaLanguage.Georgian,     "ka" },
                { CaptchaLanguage.Malay,        "ms" },
                { CaptchaLanguage.Telugu,       "te" },
                { CaptchaLanguage.Estonian,     "et" },
                { CaptchaLanguage.Malayalam,    "ml" },
                { CaptchaLanguage.Belorussian,  "be" },
                { CaptchaLanguage.Kazakh,       "kk" },
                { CaptchaLanguage.Marathi,      "mr" },
                { CaptchaLanguage.Nepali,       "ne" },
                { CaptchaLanguage.Burmese,      "my" },
                { CaptchaLanguage.Bosnian,      "bs" },
                { CaptchaLanguage.Armenian,     "hy" },
                { CaptchaLanguage.Macedonian,   "mk" },
                { CaptchaLanguage.Punjabi,      "pa" }
            };

            return dict[language];
        }
        #endregion
    }
}
