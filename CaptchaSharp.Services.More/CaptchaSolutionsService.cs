using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    public class CaptchaSolutionsService : CustomTwoCaptchaService
    {
        public CaptchaSolutionsService(string apiKey, HttpClient httpClient = null)
            : base (apiKey, new Uri("http://api.captchasolutions.com"), httpClient)
        {
            SupportedCaptchaTypes =
                CaptchaType.TextCaptcha |
                CaptchaType.ImageCaptcha |
                CaptchaType.ReCaptchaV2 |
                CaptchaType.FunCaptcha;
        }
    }
}
