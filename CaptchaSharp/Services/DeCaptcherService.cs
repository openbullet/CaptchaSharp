using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.DeCaptcher;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services
{
    public class DeCaptcherService : CaptchaService
    {
        public string Username { get; set; }
        public string Password { get; set; }
        protected HttpClient httpClient;

        public DeCaptcherService(string username, string password, HttpClient httpClient = null)
        {
            Username = username;
            Password = password;
            this.httpClient = httpClient ?? new HttpClient();
            this.httpClient.BaseAddress = new Uri("http://poster.de-captcher.com/");
            
            // Since this service replies directly with the solution to the task creation request
            // we need to set a high timeout here or it will not finish in time
            this.httpClient.Timeout = Timeout;
        }

        #region Getting the Balance
        public async override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync("",
                GetAuthPair().Add("function", "balance").ToMultipartFormDataContent(), cancellationToken)
                .ConfigureAwait(false);

            if (decimal.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal balance))
                return balance;

            throw new BadAuthenticationException(response);
        }
        #endregion

        #region Solve Methods
        public async override Task<StringResponse> SolveTextCaptchaAsync
            (string text, TextCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostMultipartToStringAsync("",
                GetAuthPair()
                .Add("function", "picture2")
                .Add("pict", text)
                .Add("pict_type", 83)
                .Add("lang", options.CaptchaLanguage.ToISO6391Code())
                .ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            if (DeCaptcherResponse.TryParse(response, out DeCaptcherResponse resp))
                return new StringResponse { Id = GetCaptchaId(resp), Response = resp.Text };

            throw new TaskSolutionException(response);
        }

        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            var content = GetAuthPair()
                .Add("function", "picture2")
                .Add("pict_type", 0)
                .Add("lang", options.CaptchaLanguage.ToISO6391Code())
                .ToMultipartFormDataContent();

            var buffer = Convert.FromBase64String(base64);
            content.Add(new ByteArrayContent(buffer), "pict");

            var response = await httpClient.PostMultipartToStringAsync("",
                content,
                cancellationToken)
                .ConfigureAwait(false);

            if (DeCaptcherResponse.TryParse(response, out DeCaptcherResponse resp))
                return new StringResponse { Id = GetCaptchaId(resp), Response = resp.Text };

            throw new TaskSolutionException(response);
        }

        public async override Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            if (proxy != null)
            {
                if (proxy.RequiresAuthentication)
                    throw new NotSupportedException("Authenticated proxies are not supported");

                if (proxy.Type != ProxyType.SOCKS4 && proxy.Type != ProxyType.SOCKS5)
                    throw new NotSupportedException("Only SOCKS proxies are supported");

                if (siteUrl.StartsWith("https"))
                    throw new NotSupportedException("Only http sites are supported");
            }

            var pairs = GetAuthPair()
                .Add("function", "proxy_url")
                .Add("url", siteUrl)
                .Add("key", siteKey);

            if (proxy != null)
                pairs.Add("proxy", $"{proxy.Host}:{proxy.Port}");

            var response = await httpClient.PostMultipartToStringAsync("",
                pairs.ToMultipartFormDataContent(),
                cancellationToken)
                .ConfigureAwait(false);

            if (DeCaptcherResponse.TryParse(response, out DeCaptcherResponse resp))
                return new StringResponse { Id = GetCaptchaId(resp), Response = resp.Text };

            throw new TaskSolutionException(response);
        }
        #endregion

        #region Reporting the solution
        public async override Task ReportSolution
            (long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            (int major, int minor) = LongToInts(id);

            await httpClient.PostMultipartToStringAsync("",
                GetAuthPair()
                .Add("function", "picture_bad2")
                .Add("major_id", major)
                .Add("minor_id", minor)
                .ToMultipartFormDataContent(), cancellationToken)
                .ConfigureAwait(false);

            // TODO: Find a way to check if the api accepted the report or not
        }
        #endregion

        #region Private Methods
        private StringPairCollection GetAuthPair()
        {
            return new StringPairCollection()
                    .Add("username", Username)
                    .Add("password", Password);
        }

        private long GetCaptchaId(DeCaptcherResponse response)
        {
            return IntsToLong(response.MajorID, response.MinorID);
        }

        // Encodes two 32-bit integers as a 64-bit long
        private long IntsToLong(int a, int b)
        {
            long l = b;
            l <<= 32;
            l |= (uint)a;
            return l;
        }

        // Gets two 32-bit integers by splitting a 64-bit long
        private (int, int) LongToInts(long longId)
        {
            return ((int)(longId & uint.MaxValue), (int)(longId >> 32));
        }
        #endregion
    }
}
