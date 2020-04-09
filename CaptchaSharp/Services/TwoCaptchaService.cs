using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;

namespace CaptchaSharp.Services
{
    public class TwoCaptchaService : CaptchaService
    {
        public string ApiKey { get; set; }

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
            var content = await GetRemoteString
                ($"http://2captcha.com/res.php?key={ApiKey}&action=getbalance&json=1", cancellationToken);

            if (decimal.TryParse(content, out decimal balance))
                return balance;

            throw new BadAuthenticationException(content);
        }

        public async override Task<CaptchaResponse> SolveRecaptchaV2Async(string siteKey, string siteUrl, CancellationToken cancellationToken = default)
        {
            var content = await GetRemoteString
                ($"http://2captcha.com/in.php?key={ApiKey}&method=userrecaptcha&googlekey={siteKey}&pageurl={siteUrl}&json=1", cancellationToken);

            if (!content.StartsWith("OK"))
                throw new TaskCreationException(content);

            var task = new CaptchaTask(content.Split('|')[1]);

            return await TryGetResult(task, cancellationToken);
        }

        protected async override Task<CaptchaResponse> CheckResult(CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var content = await GetRemoteString
                ($"http://2captcha.com/res.php?key={ApiKey}&action=get&id={task.Id}&json=1", cancellationToken);

            if (content.Contains("NOT_READY"))
                return default;

            task.Completed = true;

            if (content.Contains("ERROR"))
                throw new TaskSolutionException(content);

            return new CaptchaResponse(task.Id, content.Split('|')[1]);
        }

        private async Task<string> GetRemoteString(string url, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
