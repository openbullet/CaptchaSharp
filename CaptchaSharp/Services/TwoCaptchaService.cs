using System.Globalization;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.TwoCaptcha;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System;

namespace CaptchaSharp.Services
{
    public class TwoCaptchaService : CaptchaService
    {
        public string ApiKey { get; set; }

        private string domain = "2captcha.com";
        public string Domain
        {
            get => domain;
            set
            {
                domain = value;
                httpClient.DefaultRequestHeaders.Host = new Uri(domain).Host;
            }
        }

        private HttpClient httpClient;

        public TwoCaptchaService(string apiKey)
        {
            ApiKey = apiKey;
            httpClient = new HttpClient();
        }

        public TwoCaptchaService(string apiKey, HttpClient httpClient)
        {
            ApiKey = apiKey;
            this.httpClient = httpClient;
        }

        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetJsonAsync<TwoCaptchaResponse>
                ($"http://2captcha.com/res.php?key={ApiKey}&action=getbalance&json=1", cancellationToken);

            if (response.IsErrorCode)
                throw new BadAuthenticationException(response.Request);

            return decimal.Parse(response.Request, CultureInfo.InvariantCulture);
        }

        public async override Task<CaptchaResponse> SolveTextCaptchaAsync
            (string text, TextCaptchaOptions options = default, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("textcaptcha", text),
                    ("json", "1") }
                .Concat(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public override Task<CaptchaResponse> SolveImageCaptchaAsync
            (Bitmap image, ImageFormat format = null, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            return SolveImageCaptchaAsync(image.ToBase64(format ?? ImageFormat.Jpeg), options, cancellationToken);
        }

        public async override Task<CaptchaResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "base64"),
                    ("body", base64),
                    ("json", "1") }
                .Concat(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "userrecaptcha"),
                    ("googlekey", siteKey),
                    ("pageurl", siteUrl),
                    ("invisible", invisible.ToInt().ToString()),
                    ("json", "1") }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "userrecaptcha"),
                    ("version", "v3"),
                    ("googlekey", siteKey),
                    ("pageurl", siteUrl),
                    ("action", action),
                    ("min_score", minScore.ToString("0.0")),
                    ("json", "1") }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "funcaptcha"),
                    ("publickey", publicKey),
                    ("surl", serviceUrl),
                    ("pageurl", siteUrl),
                    ("nojs", noJS.ToInt().ToString()),
                    ("json", "1") }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveHCaptchaAsync(string siteKey, string siteUrl, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "hcaptcha"),
                    ("sitekey", siteKey),
                    ("pageurl", siteUrl),
                    ("json", "1") }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveKeyCaptchaAsync
            (string userId, string sessionId, string webServerSign1, string webServerSign2, string siteUrl, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "keycaptcha"),
                    ("s_s_c_user_id", userId),
                    ("s_s_c_session_id", sessionId),
                    ("s_s_c_web_server_sign", webServerSign1),
                    ("s_s_c_web_server_sign2", webServerSign2),
                    ("pageurl", siteUrl),
                    ("json", "1") }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveGeeTestAsync
            (string gt, string challenge, string apiServer, string siteUrl, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "geetest"),
                    ("gt", gt),
                    ("challenge", challenge),
                    ("api_server", apiServer),
                    ("pageurl", siteUrl),
                    ("json", "1") }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        private async Task<CaptchaResponse> TryGetResult
            (TwoCaptchaResponse response, CancellationToken cancellationToken = default)
        {
            if (response.IsErrorCode)
                throw new TaskCreationException(response.Request);

            var task = new CaptchaTask(response.Request);

            return await TryGetResult(task, cancellationToken).ConfigureAwait(false);
        }

        protected async override Task<CaptchaResponse> CheckResult(CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/res.php?key={ApiKey}&action=get&id={task.Id}&json=1", cancellationToken).ConfigureAwait(false);

            if (!response.Success && response.Request == "CAPCHA_NOT_READY")
                return default;

            task.Completed = true;

            if (response.IsErrorCode)
                throw new TaskSolutionException(response.Request);

            return new CaptchaResponse(task.Id, response.Request);
        }

        public async override Task ReportSolution(string taskId, bool correct = false, CancellationToken cancellationToken = default)
        {
            var action = correct ? "reportgood" : "reportbad";

            var response = await httpClient.GetJsonAsync<TwoCaptchaResponse>
                ($"http://{Domain}/res.php?key={ApiKey}&action={action}&id={taskId}&json=1", cancellationToken).ConfigureAwait(false);

            if (response.IsErrorCode)
                throw new TaskReportException(response.Request);
        }

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

        protected override IEnumerable<(string, string)> ConvertCapabilities(TextCaptchaOptions options)
        {
            // If null, don't return any parameters
            if (options == null)
                return new (string, string)[] { };

            var capabilities = new List<(string, string)>();

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.LanguageGroup))
                capabilities.Add(("language", ((int)options.CaptchaLanguageGroup).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Language))
                capabilities.Add(("lang", GetLanguageCode(options.CaptchaLanguage)));

            return capabilities;
        }

        protected override IEnumerable<(string, string)> ConvertCapabilities(ImageCaptchaOptions options)
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

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Language))
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
