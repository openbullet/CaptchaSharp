using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    public class CaptchasIOService : CustomTwoCaptchaService
    {
        public CaptchasIOService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("https://api.captchas.io"), httpClient) 
        {
            SupportedCaptchaTypes =
                CaptchaType.ImageCaptcha |
                CaptchaType.ReCaptchaV2 |
                CaptchaType.ReCaptchaV3;
        }
    }
}
