using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    /// <summary>The service provided by <c>https://azcaptcha.com/</c></summary>
    public class AzCaptchaService : CustomTwoCaptchaService
    {
        /// <summary>Initializes a <see cref="AzCaptchaService"/> using the given <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public AzCaptchaService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("http://azcaptcha.com"), httpClient, false)
        {
            SupportedCaptchaTypes =
                CaptchaType.ImageCaptcha |
                CaptchaType.ReCaptchaV2 |
                CaptchaType.ReCaptchaV3;
        }
    }
}
