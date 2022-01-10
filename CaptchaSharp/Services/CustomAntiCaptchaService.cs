using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services
{
    /// <summary>The service provided by a service that implements the anti-captcha API.</summary>
    public class CustomAntiCaptchaService : AntiCaptchaService
    {
        /// <summary>Initializes a <see cref="CustomAntiCaptchaService"/> using the given <paramref name="apiKey"/>, 
        /// <paramref name="baseUri"/> and <paramref name="httpClient"/>.
        /// If <paramref name="httpClient"/> is null, a default one will be created.
        /// </summary>
        public CustomAntiCaptchaService(string apiKey, Uri baseUri, HttpClient httpClient = null)
            : base(apiKey, httpClient)
        {
            SetupHttpClient(baseUri);
        }

        /// <summary>Sets anti-captcha.com as host and <paramref name="baseUri"/> as <see cref="HttpClient.BaseAddress"/> 
        /// for the <see cref="HttpClient"/> requests.</summary>
        protected void SetupHttpClient(Uri baseUri)
        {
            httpClient.BaseAddress = baseUri;
        }

        #region Supported Types
        /// <summary>The supported captcha types for this service.</summary>
        public CaptchaType SupportedCaptchaTypes { get; set; } =
            CaptchaType.ImageCaptcha |
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3 |
            CaptchaType.FunCaptcha |
            CaptchaType.HCaptcha |
            CaptchaType.GeeTest;
        #endregion
    }
}
