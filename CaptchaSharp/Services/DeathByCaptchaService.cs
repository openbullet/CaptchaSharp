using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services
{
    public class DeathByCaptchaService : CustomTwoCaptchaService
{ 
        public DeathByCaptchaService(string username, string password, HttpClient httpClient = null) 
            : base($"{username}:{password}", new Uri("http://api.deathbycaptcha.com"), httpClient) 
        {
            SupportedCaptchaTypes =
                CaptchaType.TextCaptcha |
                CaptchaType.ImageCaptcha |
                CaptchaType.ReCaptchaV2 |
                CaptchaType.ReCaptchaV3 |
                CaptchaType.FunCaptcha;
        }
    }
}
