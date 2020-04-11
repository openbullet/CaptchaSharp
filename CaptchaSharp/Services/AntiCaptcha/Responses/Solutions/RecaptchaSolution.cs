using CaptchaSharp.Models;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions
{
    internal class RecaptchaSolution : ITaskSolution
    {
        public string GRecaptchaResponse { get; set; }

        public CaptchaResponse ToCaptchaResponse(int id)
        {
            return new StringResponse
            {
                Id = id,
                Response = GRecaptchaResponse
            };
        }
    }
}
