using CaptchaSharp.Models;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions
{
    internal class FuncaptchaSolution : ITaskSolution
    {
        public string Token { get; set; }

        public CaptchaResponse ToCaptchaResponse(int id)
        {
            return new StringResponse
            {
                Id = id,
                Response = Token
            };
        }
    }
}
