using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services
{
    public class CustomTwoCaptchaService : CaptchaService
    {
        public string ApiKey { get; set; }
        private HttpClient httpClient;

        // The baseUri must end with a forward slash
        public CustomTwoCaptchaService(string apiKey, Uri baseUri, HttpClient httpClient = null)
        {
            ApiKey = apiKey;
            this.httpClient = httpClient ?? new HttpClient();

            // Use 2captcha.com as host header to simulate an entry in the hosts file
            this.httpClient.DefaultRequestHeaders.Host = "2captcha.com";
            this.httpClient.BaseAddress = baseUri;
        }

        /*
         * IMPORTANT:
         * Services that implement the 2captcha API don't always support JSON responses so we will not set the json=1 flag
         * 
         */

        #region Getting the Balance
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ($"res.php?key={ApiKey}&action=getbalance", cancellationToken);

            if (IsErrorCode(response))
                throw new BadAuthenticationException(response);

            return decimal.Parse(response, CultureInfo.InvariantCulture);
        }
        #endregion

        #region Solve Methods
        public async override Task<CaptchaResponse> SolveTextCaptchaAsync
            (string text, TextCaptchaOptions options = default, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("textcaptcha", text) }
                .Concat(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "base64"),
                    ("body", base64) }
                .Concat(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "userrecaptcha"),
                    ("googlekey", siteKey),
                    ("pageurl", siteUrl),
                    ("invisible", invisible.ToInt().ToString()) }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "userrecaptcha"),
                    ("version", "v3"),
                    ("googlekey", siteKey),
                    ("pageurl", siteUrl),
                    ("action", action),
                    ("min_score", minScore.ToString("0.0")) }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "funcaptcha"),
                    ("publickey", publicKey),
                    ("surl", serviceUrl),
                    ("pageurl", siteUrl),
                    ("nojs", noJS.ToInt().ToString()) }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveHCaptchaAsync
            (string siteKey, string siteUrl, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "hcaptcha"),
                    ("sitekey", siteKey),
                    ("pageurl", siteUrl) }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveKeyCaptchaAsync
            (string userId, string sessionId, string webServerSign1, string webServerSign2, string siteUrl, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "keycaptcha"),
                    ("s_s_c_user_id", userId),
                    ("s_s_c_session_id", sessionId),
                    ("s_s_c_web_server_sign", webServerSign1),
                    ("s_s_c_web_server_sign2", webServerSign2),
                    ("pageurl", siteUrl) }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<CaptchaResponse> SolveGeeTestAsync
            (string gt, string challenge, string apiServer, string siteUrl, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartAsync
                ("in.php",
                new (string, string)[] {
                    ("key", ApiKey),
                    ("method", "geetest"),
                    ("gt", gt),
                    ("challenge", challenge),
                    ("api_server", apiServer),
                    ("pageurl", siteUrl) }
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(response, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Getting the result
        private async Task<CaptchaResponse> TryGetResult
            (string response, CancellationToken cancellationToken = default)
        {
            if (IsErrorCode(response))
                throw new TaskCreationException(response);

            var task = new CaptchaTask(TakeSecondSlice(response));

            return await TryGetResult(task, cancellationToken).ConfigureAwait(false);
        }

        protected async override Task<CaptchaResponse> CheckResult(CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ($"res.php?key={ApiKey}&action=get&id={task.Id}", cancellationToken).ConfigureAwait(false);

            if (response.Contains("CAPCHA_NOT_READY"))
                return default;

            task.Completed = true;

            if (IsErrorCode(response))
                throw new TaskSolutionException(response);

            return new CaptchaResponse(task.Id, TakeSecondSlice(response));
        }
        #endregion

        #region Reporting the solution
        public async override Task ReportSolution(string taskId, bool correct = false, CancellationToken cancellationToken = default)
        {
            var action = correct ? "reportgood" : "reportbad";

            var response = await httpClient.GetStringAsync
                ($"res.php?key={ApiKey}&action={action}&id={taskId}", cancellationToken).ConfigureAwait(false);

            if (IsErrorCode(response))
                throw new TaskReportException(response);
        }
        #endregion

        #region Utility methods
        private bool IsErrorCode(string response)
        {
            return !response.StartsWith("OK");
        }

        private string TakeSecondSlice(string str)
        {
            return str.Split('|')[1];
        }
        #endregion
    }
}
