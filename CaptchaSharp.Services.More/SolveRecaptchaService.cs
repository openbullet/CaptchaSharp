using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    public class SolveRecaptchaService : CustomTwoCaptchaService
    {
        public SolveRecaptchaService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("http://api.solverecaptcha.com"), httpClient)
        {
            SupportedCaptchaTypes =
                CaptchaType.ReCaptchaV2 |
                CaptchaType.ReCaptchaV3;
        }
    }
}
