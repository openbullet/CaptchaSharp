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
            this.httpClient.BaseAddress = new Uri("https://api.apitruecaptcha.org/");
            this.httpClient.Timeout = Timeout;
        }

        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetStringAsync
                ("one/getbalance",
                new StringPairCollection()
                .Add("username", UserId)
                .Add("apikey", ApiKey),
                cancellationToken)
                .ConfigureAwait(false);

            if (decimal.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal balance))
                return balance;

            else
                throw new BadAuthenticationException(response);
        }

        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var content = new JObject();
            content.Add("userid", UserId);
            content.Add("apikey", ApiKey);
            content.Add("data", base64);

            var response = await httpClient.PostJsonToStringAsync
                ("one/gettext",
                content,
                cancellationToken, false)
                .ConfigureAwait(false);

            var jObject = JObject.Parse(response);
            return new StringResponse { Id = 0, Response = jObject["result"].ToString() };
        }
    }
}
