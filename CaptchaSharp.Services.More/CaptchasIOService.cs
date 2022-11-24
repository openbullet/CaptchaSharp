using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    /// <summary>The service provided by <c>https://captchas.io/</c></summary>
    public class CaptchasIOService : CustomTwoCaptchaService
    {
        /// <summary>Initializes a <see cref="CaptchasIOService"/> using the given <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public CaptchasIOService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("https://api.captchas.io"), httpClient, false) 
        {
            SupportedCaptchaTypes =
                CaptchaType.ImageCaptcha |
                CaptchaType.ReCaptchaV2 |
                CaptchaType.ReCaptchaV3;
        }
    }
}
