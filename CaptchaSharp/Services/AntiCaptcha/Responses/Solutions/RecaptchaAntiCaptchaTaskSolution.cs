using CaptchaSharp.Models;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions
{
    internal class RecaptchaAntiCaptchaTaskSolution : AntiCaptchaTaskSolution
    {
        public string GRecaptchaResponse { get; set; }

        public override CaptchaResponse ToCaptchaResponse(long id)
        {
            return new StringResponse
            {
                Id = id,
                Response = GRecaptchaResponse
            };
        }
    }
}
