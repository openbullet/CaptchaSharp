using System;
using System.Net.Http;

namespace CaptchaSharp.Services
{
    public class RuCaptchaService : CustomTwoCaptchaService
    {
        public RuCaptchaService(string apiKey, HttpClient httpClient = null)
            : base(apiKey, new Uri("http://rucaptcha.com"), httpClient) { }
    }
}
