using System;
using CaptchaSharp.Enums;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    /// <summary>The service provided by <c>https://solvecaptcha.com/</c></summary>
    public class SolveCaptchaService : CustomTwoCaptchaService
    {
        /// <summary>Initializes a <see cref="SolveCaptchaService"/> using the given <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public SolveCaptchaService(string apiKey, HttpClient? httpClient = null)
            : base(apiKey, new Uri("http://api.solvecaptcha.com"), httpClient, false)
        {
            SupportedCaptchaTypes =
                CaptchaType.TextCaptcha |
                CaptchaType.ImageCaptcha |
                CaptchaType.ReCaptchaV2 |
                CaptchaType.FunCaptcha |
                CaptchaType.KeyCaptcha;
        }
    }
}
