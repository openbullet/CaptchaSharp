using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    /// <summary>The service provided by <c>https://anycaptcha.com/</c></summary>
    public class AnyCaptchaService : CustomAntiCaptchaService
    {
        /// <summary>Initializes a <see cref="AnyCaptchaService"/> using the given <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public AnyCaptchaService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("https://api.anycaptcha.com"), httpClient)
        {
            SupportedCaptchaTypes =
                CaptchaType.ImageCaptcha |
                CaptchaType.ReCaptchaV2 |
                CaptchaType.ReCaptchaV3 |
                CaptchaType.FunCaptcha |
                CaptchaType.HCaptcha;
        }
    }
}
