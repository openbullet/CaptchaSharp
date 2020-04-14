using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services
{
    public class TrueCaptchaService : CaptchaService
    {
        public string UserId { get; set; }
        public string ApiKey { get; set; }
        protected HttpClient httpClient;

        public TrueCaptchaService(string username, string apiKey, HttpClient httpClient = null)
        {
            UserId = username;
            ApiKey = apiKey;

            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("https://api.apitruecaptcha.org/one");
            this.httpClient.Timeout = Timeout;
        }

        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("getbalance",
                GetAuthPair(),
                cancellationToken)
                .ConfigureAwait(false);

            var jObject = JObject.Parse(response);

            try
            {
                return decimal.Parse(jObject["balance"].ToString(), CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new BadAuthenticationException(response);
            }
        }

        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostToStringAsync
                ("gettext",
                GetAuthPair()
                .Add("data", base64),
                cancellationToken)
                .ConfigureAwait(false);

            var jObject = JObject.Parse(response);
            return new StringResponse { Id = 0, Response = jObject["result"].ToString() };
        }

        public async override Task ReportSolution
            (long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private StringPairCollection GetAuthPair()
            => new StringPairCollection().Add("userid", UserId).Add("apikey", ApiKey);
    }
}
