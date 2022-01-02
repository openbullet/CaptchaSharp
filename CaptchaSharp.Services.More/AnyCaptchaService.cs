using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    public class AnyCaptchaService : CustomAntiCaptchaService
    {
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
