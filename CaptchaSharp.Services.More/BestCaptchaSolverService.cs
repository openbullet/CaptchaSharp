using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    public class BestCaptchaSolverService : CustomTwoCaptchaService
    {
        public BestCaptchaSolverService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("http://bcsapi.xyz"), httpClient)
        {
            SupportedCaptchaTypes =
                CaptchaType.ImageCaptcha |
                CaptchaType.ReCaptchaV2;
        }
    }
}
