using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services
{
    /// <summary>The service provided by <c>https://www.9kw.eu/</c></summary>
    public class NineKWService : CaptchaService
    {
        /// <summary>Your secret api key.</summary>
        public string ApiKey { get; set; }

        /// <summary>The default <see cref="HttpClient"/> used for requests.</summary>
        private HttpClient httpClient;

        /// <summary>Initializes a <see cref="ImageTyperzService"/> using the given <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public NineKWService(string apiKey, HttpClient httpClient = null)
        {
            ApiKey = apiKey;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("https://www.9kw.eu/");

            // Since this service replies directly with the solution to the task creation request (for image captchas)
            // we need to set a high timeout here or it will not finish in time
            this.httpClient.Timeout = Timeout;
        }

        #region Getting the Balance
        /// <inheritdoc/>
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("index.cgi",
                GetAuthPair()
                .Add("action", "usercaptchaguthaben"),
                cancellationToken)
                .ConfigureAwait(false);

            if (IsError(response))
                throw new BadAuthenticationException(GetErrorMessage(response));

            return decimal.Parse(response, CultureInfo.InvariantCulture);
        }
        #endregion

        #region Solve Methods
        /// <inheritdoc/>
        public async override Task<StringResponse> SolveTextCaptchaAsync
            (string text, TextCaptchaOptions options = default, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("file-upload-01", text)
                    .Add("textonly", 1)
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            if (IsError(response))
                throw new TaskSolutionException(GetErrorMessage(response));

            return await TryGetResult(response, CaptchaType.TextCaptcha, cancellationToken).ConfigureAwait(false) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync
                ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("file-upload-01", base64)
                    .Add("base64", 1)
                    .Add(ConvertCapabilities(options))
                    .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            if (IsError(response))
                throw new TaskSolutionException(GetErrorMessage(response));

            return await TryGetResult(response, CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("interactive", 1)
                    .Add("oldsource", "recaptchav2")
                    .Add("file-upload-01", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, bool enterprise = false,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("interactive", 1)
                    .Add("oldsource", "recaptchav3")
                    .Add("file-upload-01", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("interactive", 1)
                    .Add("oldsource", "funcaptcha")
                    .Add("file-upload-01", publicKey)
                    .Add("pageurl", siteUrl)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveHCaptchaAsync
            (string siteKey, string siteUrl, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("interactive", 1)
                    .Add("oldsource", "hcaptcha")
                    .Add("file-upload-01", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false) as StringResponse;
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

        /// <inheritdoc/>
        protected async override Task<CaptchaResponse> CheckResult
            (CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                    ("index.cgi",
                    GetAuthPair()
                    .Add("action", "usercaptchacorrectdata")
                    .Add("id", task.Id),
                    cancellationToken)
                    .ConfigureAwait(false);

            // Not solved yet
            if (string.IsNullOrEmpty(response) || response.Contains("CAPTCHA_NOT_READY"))
                return default;

            task.Completed = true;

            if (IsError(response) || response.Contains("ERROR_NO_USER"))
                throw new TaskSolutionException(response);

            return new StringResponse { Id = task.Id, Response = response };
        }
        #endregion

        #region Reporting the solution
        /// <inheritdoc/>
        public async override Task ReportSolution
            (long taskId, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchacorrectback")
                    .Add("id", taskId.ToString())
                    .Add("correct", correct ? 1 : 2),
                cancellationToken);

            if (IsError(response))
                throw new TaskReportException(response);
        }
        #endregion

        #region Private Methods
        private StringPairCollection GetAuthPair()
            => new StringPairCollection().Add("apikey", ApiKey);

        private bool IsError(string response)
            => Regex.IsMatch(response, @"^\d{4} ");

        private string GetErrorMessage(string response)
            => Regex.Replace(response, @"^\d{4} ", "");
        #endregion

        #region Proxies
        /// <summary></summary>
        protected IEnumerable<(string, string)> ConvertProxy(Proxy proxy)
        {
            if (proxy == null)
                return new (string, string)[] { };

            return new (string, string)[]
            {
                ("proxy", $"{proxy.Host}:{proxy.Port}"),
                ("proxytype", proxy.Type.ToString().ToLower())
            };
        }
        #endregion

        #region Capabilities
        /// <inheritdoc/>
        public new CaptchaServiceCapabilities Capabilities =>
            CaptchaServiceCapabilities.Phrases |
            CaptchaServiceCapabilities.CaseSensitivity |
            CaptchaServiceCapabilities.CharacterSets |
            CaptchaServiceCapabilities.Calculations |
            CaptchaServiceCapabilities.MinLength |
            CaptchaServiceCapabilities.MaxLength |
            CaptchaServiceCapabilities.Instructions;

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
                capabilities.Add(("case-sensitive", Convert.ToInt32(options.CaseSensitive).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.CharacterSets))
            {
                int charSet = 0;

                if (options.CharacterSet != CharacterSet.OnlyNumbersOrOnlyLetters)
                {
                    switch (options.CharacterSet)
                    {
                        case CharacterSet.OnlyNumbers:
                            charSet = 1;
                            break;

                        case CharacterSet.OnlyLetters:
                            charSet = 2;
                            break;

                        case CharacterSet.BothNumbersAndLetters:
                            charSet = 3;
                            break;

                        default:
                            break;
                    }
                }

                capabilities.Add(("numeric", charSet.ToString()));
            }

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Calculations))
                capabilities.Add(("math", Convert.ToInt32(options.RequiresCalculation).ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.MinLength))
                capabilities.Add(("min_len", options.MinLength.ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.MaxLength))
                capabilities.Add(("max_len", options.MaxLength.ToString()));

            if (Capabilities.HasFlag(CaptchaServiceCapabilities.Instructions))
                capabilities.Add(("textinstructions", options.TextInstructions));

            return capabilities;
        }
        #endregion
    }
}
