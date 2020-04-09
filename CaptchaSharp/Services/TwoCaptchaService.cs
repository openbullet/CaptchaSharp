using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.TwoCaptcha;
using Newtonsoft.Json;

namespace CaptchaSharp.Services
{
    public class TwoCaptchaService : CaptchaService
    {
        public new CaptchaServiceCapabilities Capabilities =
            CaptchaServiceCapabilities.LanguageGroup |
            CaptchaServiceCapabilities.Language |
            CaptchaServiceCapabilities.Phrases |
            CaptchaServiceCapabilities.CaseSensitivity |
            CaptchaServiceCapabilities.CharacterSets |
            CaptchaServiceCapabilities.Calculations |
            CaptchaServiceCapabilities.MinLength |
            CaptchaServiceCapabilities.MaxLength |
            CaptchaServiceCapabilities.Instructions;

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
            var response = await httpClient.GetJsonAsync<TwoCaptchaResponse>
                ($"http://2captcha.com/res.php?key={ApiKey}&action=getbalance&json=1", cancellationToken);

            if (response.IsErrorCode)
                throw new BadAuthenticationException(response.Request);

            return decimal.Parse(response.Request, CultureInfo.InvariantCulture);
        }

        public async override Task<CaptchaResponse> SolveRecaptchaV2Async(string siteKey, string siteUrl, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetJsonAsync<TwoCaptchaResponse>
                ($"http://2captcha.com/in.php?key={ApiKey}&method=userrecaptcha&googlekey={siteKey}&pageurl={siteUrl}&json=1", cancellationToken);

            if (response.IsErrorCode)
                throw new TaskCreationException(response.Request);

            var task = new CaptchaTask(response.Request);

            return await TryGetResult(task, cancellationToken);
        }

        protected async override Task<CaptchaResponse> CheckResult(CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetJsonAsync<TwoCaptchaResponse>
                ($"http://2captcha.com/res.php?key={ApiKey}&action=get&id={task.Id}&json=1", cancellationToken);

            if (!response.Success && response.Request.Contains("NOT_READY"))
                return default;

            task.Completed = true;

            if (response.IsErrorCode)
                throw new TaskSolutionException(response.Request);

            return new CaptchaResponse(task.Id, response.Request);
        }
    }
}
