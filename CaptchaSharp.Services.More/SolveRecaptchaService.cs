using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services.More
{
    /// <summary>The service provided by <c>https://solverecaptcha.com/</c></summary>
    public class SolveRecaptchaService : CustomTwoCaptchaService
    {
        /// <summary>Initializes a <see cref="SolveRecaptchaService"/> using the given <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public SolveRecaptchaService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("http://api.solverecaptcha.com"), httpClient)
        {
            this.httpClient.DefaultRequestHeaders.Host = "api.solverecaptcha.com";
            this.httpClient.Timeout = Timeout;

            SupportedCaptchaTypes =
                CaptchaType.ReCaptchaV2 |
                CaptchaType.ReCaptchaV3;
        }

        /// <inheritdoc/>
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            // There is no balance since this service has a monthly subscription
            return await Task.Run(() => 999).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("",
                GetAuthPair()
                .Add("sitekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("version", invisible ? "invisible" : "v2")
                .Add("invisible", Convert.ToInt32(invisible).ToString())
                .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            if (IsErrorCode(response))
                ThrowException(response);

            return new StringResponse { Id = 0, Response = TakeSecondSlice(response) };
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, bool enterprise = false,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("",
                GetAuthPair()
                .Add("sitekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("action", action)
                .Add("min_score", minScore.ToString("0.0", CultureInfo.InvariantCulture))
                .Add("version", "v3")
                .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            if (IsErrorCode(response))
                ThrowException(response);

            return new StringResponse { Id = 0, Response = TakeSecondSlice(response) };
        }

        /// <inheritdoc/>
        public override Task ReportSolution
            (long taskId, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        private StringPairCollection GetAuthPair()
            => new StringPairCollection().Add("key", ApiKey);

        private void ThrowException(string response)
        {
            switch (response)
            {
                case "ERROR_API_KEY_NOT_FOUND":
                case "ERROR_ACCESS_DENIED":
                    throw new BadAuthenticationException(response);

                case "ERROR_NO_AVAILABLE_THREADS":
                    throw new TaskCreationException(response);

                case "ERROR_CAPTCHA_UNSOLVABLE":
                    throw new TaskSolutionException(response);

                default:
                    throw new Exception(response);
            }
        }
    }
}
