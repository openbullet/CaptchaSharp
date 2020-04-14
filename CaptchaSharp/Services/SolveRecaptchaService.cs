using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services
{
    public class SolveRecaptchaService : CustomTwoCaptchaService
    {
        public SolveRecaptchaService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("http://api.solverecaptcha.com"), httpClient)
        {
            httpClient.DefaultRequestHeaders.Host = "api.solverecaptcha.com";

            SupportedCaptchaTypes =
                CaptchaType.ReCaptchaV2 |
                CaptchaType.ReCaptchaV3;
        }

        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            // There is no balance since this service has a monthly subscription
            return await Task.Run(() => 999).ConfigureAwait(false);
        }

        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("",
                GetAuthPair()
                .Add("sitekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("version", invisible ? "invisible" : "v2")
                .Add("invisible", invisible.ToInt().ToString())
                .Add(ConvertProxy(proxy)),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, CaptchaType.ReCaptchaV2, cancellationToken) as StringResponse;
        }

        public async override Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, Proxy proxy = null,
            CancellationToken cancellationToken = default)
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

            return await TryGetResult(response, CaptchaType.ReCaptchaV3, cancellationToken) as StringResponse;
        }

        public async override Task ReportSolution
            (long taskId, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        private StringPairCollection GetAuthPair()
            => new StringPairCollection().Add("key", ApiKey);
    }
}
