using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services
{
    public class AzCaptchaService : CustomTwoCaptchaService
    {
        public AzCaptchaService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("http://azcaptcha.com"), httpClient) 
        {
            SupportedCaptchaTypes =
                CaptchaType.ImageCaptcha |
                CaptchaType.ReCaptchaV2 |
                CaptchaType.ReCaptchaV3;
        }
    }
}
