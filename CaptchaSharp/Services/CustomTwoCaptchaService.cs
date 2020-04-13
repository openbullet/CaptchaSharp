using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services
{
    public class CustomTwoCaptchaService : TwoCaptchaService
    {
        // The baseUri must end with a forward slash
        public CustomTwoCaptchaService(string apiKey, Uri baseUri, HttpClient httpClient = null) : base(apiKey, httpClient)
        {
            SetupHttpClient(baseUri);

            // Services that implement the 2captcha API don't always support
            // JSON responses so we will not set the json=1 flag
            UseJsonFlag = false;
        }

        protected void SetupHttpClient(Uri baseUri)
        {
            // Use 2captcha.com as host header to simulate an entry in the hosts file
            httpClient.DefaultRequestHeaders.Host = "2captcha.com";
            httpClient.BaseAddress = baseUri;
        }

        #region Supported Types
        public CaptchaType SupportedCaptchaTypes { get; set; } =
            CaptchaType.TextCaptcha |
            CaptchaType.ImageCaptcha |
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3 |
            CaptchaType.FunCaptcha |
            CaptchaType.HCaptcha |
            CaptchaType.KeyCaptcha |
            CaptchaType.GeeTest;
        #endregion
    }
}
