using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;

namespace CaptchaSharp.Services.More
{
    /// <summary>The service provided by <c>https://apitruecaptcha.org/</c></summary>
    public class TrueCaptchaService : CaptchaService
    {
        /// <summary>Your user id.</summary>
        public string UserId { get; set; }

        /// <summary>Your secret api key.</summary>
        public string ApiKey { get; set; }

        /// <summary>The default <see cref="HttpClient"/> used for requests.</summary>
        protected HttpClient httpClient;

        /// <summary>Initializes a <see cref="TrueCaptchaService"/> using the given <paramref name="userId"/>, <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public TrueCaptchaService(string userId, string apiKey, HttpClient httpClient = null)
        {
            UserId = userId;
            ApiKey = apiKey;

            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("https://api.apitruecaptcha.org/");
            this.httpClient.Timeout = Timeout;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
                camelizeKeys: false,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var jObject = JObject.Parse(response);
            return new StringResponse { Id = 0, Response = jObject["result"].ToString() };
        }
    }
}
