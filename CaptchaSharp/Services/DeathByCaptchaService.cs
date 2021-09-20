using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.DeathByCaptcha.Tasks;
using CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CaptchaSharp.Services
{
    /// <summary>The service provided by <c>https://www.deathbycaptcha.com/</c></summary>
    public class DeathByCaptchaService : CaptchaService
    {
        /// <summary>Your DeathByCaptcha account name.</summary>
        public string Username { get; set; }

        /// <summary>Your DeathByCaptcha account password.</summary>
        public string Password { get; set; }

        /// <summary>The default <see cref="HttpClient"/> used for requests.</summary>
        protected HttpClient httpClient;

        /*
         * Sometimes the DBC API randomly replies with query strings even when json is requested, so
         * we will avoid using the Accept: application/json header.
         */

        /// <summary>Initializes a <see cref="DeathByCaptchaService"/> using the given account credentials and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public DeathByCaptchaService(string username, string password, HttpClient httpClient = null)
        {
            Username = username;
            Password = password;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("http://api.dbcapi.me/api/");
        }

        #region Getting the Balance
        /// <inheritdoc/>
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsync(
                "user",
                GetAuthPair(),
                cancellationToken)
                .ConfigureAwait(false);

            var query = HttpUtility.ParseQueryString(await DecodeIsoResponse(response));

            if (IsError(query))
                throw new BadAuthenticationException(GetErrorMessage(query));

            // The server returns the balance in cents
            return decimal.Parse(query["balance"], CultureInfo.InvariantCulture) / 100;
        }
        #endregion

        #region Solve Methods
        /// <inheritdoc/>
        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsync
                ("captcha",
                GetAuthPair()
                    .Add("captchafile", $"base64:{base64}")
                    .ToMultipartFormDataContent(),
                cancellationToken);

            return await TryGetResult(HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
                CaptchaType.ImageCaptcha, cancellationToken) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            DBCTaskProxyless task;

            if (proxy != null)
            {
                task = new RecaptchaV2Task
                {
                    GoogleKey = siteKey,
                    PageUrl = siteUrl
                }.SetProxy(proxy);
            }
            else
            {
                task = new RecaptchaV2TaskProxyless
                {
                    GoogleKey = siteKey,
                    PageUrl = siteUrl
                };
            }

            var response = await httpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 4)
                    .Add("token_params", task.SerializeLowerCase()),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
                CaptchaType.ReCaptchaV2, cancellationToken) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, bool enterprise = false,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            DBCTaskProxyless task;

            if (proxy != null)
            {
                task = new RecaptchaV3Task
                {
                    GoogleKey = siteKey,
                    PageUrl = siteUrl,
                    Action = action,
                    Min_Score = minScore
                }.SetProxy(proxy);
            }
            else
            {
                task = new RecaptchaV3TaskProxyless
                {
                    GoogleKey = siteKey,
                    PageUrl = siteUrl,
                    Action = action,
                    Min_Score = minScore
                };
            }

            var response = await httpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 5)
                    .Add("token_params", task.SerializeLowerCase()),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
                CaptchaType.ReCaptchaV3, cancellationToken) as StringResponse;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            DBCTaskProxyless task;

            if (proxy != null)
            {
                task = new FuncaptchaTask
                {
                    PublicKey = publicKey,
                    PageUrl = siteUrl
                }.SetProxy(proxy);
            }
            else
            {
                task = new FuncaptchaTaskProxyless
                {
                    PublicKey = publicKey,
                    PageUrl = siteUrl
                };
            }

            var response = await httpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 6)
                    .Add("funcaptcha_params", task.SerializeLowerCase()),
                cancellationToken)
                .ConfigureAwait(false);

            return await TryGetResult(HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
                CaptchaType.FunCaptcha, cancellationToken) as StringResponse;
        }
        #endregion

        #region Getting the result
        private async Task<CaptchaResponse> TryGetResult
            (NameValueCollection response, CaptchaType type, CancellationToken cancellationToken = default)
        {
            if (IsError(response))
                throw new TaskCreationException(GetErrorMessage(response));

            var task = new CaptchaTask(response["captcha"], type);

            return await TryGetResult(task, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected async override Task<CaptchaResponse> CheckResult(CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync($"captcha/{task.Id}", cancellationToken);
            var query = HttpUtility.ParseQueryString(await DecodeIsoResponse(response));

            if (query["text"] == string.Empty)
                return default;

            task.Completed = true;

            if (IsError(query) || query["is_correct"] == "0")
                throw new TaskSolutionException(GetErrorMessage(query));

            return new StringResponse() { Id = task.Id, Response = query["text"] };
        }
        #endregion

        #region Reporting the solution
        /// <inheritdoc/>
        public async override Task ReportSolution
            (long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            if (correct)
                throw new NotSupportedException("This service doesn't allow reporting of good solutions");

            var response = await httpClient.PostAsync(
                $"captcha/{id}/report",
                GetAuthPair(),
                cancellationToken)
                .ConfigureAwait(false);

            var query = HttpUtility.ParseQueryString(await DecodeIsoResponse(response));

            if (IsError(query))
                throw new TaskReportException(GetErrorMessage(query));
        }
        #endregion

        #region Private Methods
        private async Task<string> DecodeIsoResponse(HttpResponseMessage response)
        {
            using (var sr = new StreamReader(
                await response.Content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1")))
            {
                return sr.ReadToEnd();
            }
        }

        private StringPairCollection GetAuthPair()
        {
            return new StringPairCollection()
                    .Add("username", Username)
                    .Add("password", Password);
        }

        private bool IsError(NameValueCollection response)
        {
            return response["status"] == "255";
        }

        private string GetErrorMessage(NameValueCollection response)
        {
            return response["error"];
        }
        #endregion
    }
}
