using System;
using CaptchaSharp.Enums;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    public class SolveCaptchaService : CustomTwoCaptchaService
    {
        public SolveCaptchaService(string apiKey, HttpClient httpClient = null)
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
