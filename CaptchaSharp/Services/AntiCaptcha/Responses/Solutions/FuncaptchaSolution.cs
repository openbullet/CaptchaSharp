using CaptchaSharp.Models;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions
{
    internal class FuncaptchaSolution : Solution
    {
        public string Token { get; set; }

        public override CaptchaResponse ToCaptchaResponse(long id)
        {
            return new StringResponse
            {
                Id = id,
                Response = Token
            };
        }
    }
}
