using CaptchaSharp.Models;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions
{
    internal class FuncaptchaAntiCaptchaTaskSolution : AntiCaptchaTaskSolution
    {
        public string Token { get; set; }

        public override CaptchaResponse ToCaptchaResponse(string id)
        {
            return new StringResponse
            {
                Id = id,
                Response = Token
            };
        }
    }
}
