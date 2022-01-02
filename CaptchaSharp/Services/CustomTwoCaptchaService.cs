using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services
{
    /// <summary>The service provided by a service that implements the 2captcha API.</summary>
    public class CustomTwoCaptchaService : TwoCaptchaService
    {
        /// <summary>Initializes a <see cref="CustomTwoCaptchaService"/> using the given <paramref name="apiKey"/>, 
        /// <paramref name="baseUri"/> and <paramref name="httpClient"/>.
        /// If <paramref name="httpClient"/> is null, a default one will be created.
        /// If <paramref name="overrideHostHeader"/> is true, the Host header will be changed to 2captcha.com</summary>
        public CustomTwoCaptchaService(string apiKey, Uri baseUri, HttpClient httpClient = null, bool overrideHostHeader = true)
            : base(apiKey, httpClient)
        {
            SetupHttpClient(baseUri, overrideHostHeader);

            // Services that implement the 2captcha API don't always support
            // JSON responses so we will not set the json=1 flag
            UseJsonFlag = false;
        }

        /// <summary>Sets 2captcha.com as host and <paramref name="baseUri"/> as <see cref="HttpClient.BaseAddress"/> 
        /// for the <see cref="HttpClient"/> requests.</summary>
        protected void SetupHttpClient(Uri baseUri, bool overrideHostHeader = true)
        {
            if (overrideHostHeader)
            {
                // Use 2captcha.com as host header to simulate an entry in the hosts file
                httpClient.DefaultRequestHeaders.Host = "2captcha.com";
            }
            
            httpClient.BaseAddress = baseUri;
        }

        #region Supported Types
        /// <summary>The supported captcha types for this service.</summary>
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
