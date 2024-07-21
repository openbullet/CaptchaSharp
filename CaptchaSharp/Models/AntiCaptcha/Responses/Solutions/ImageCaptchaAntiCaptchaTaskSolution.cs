using CaptchaSharp.Models;

namespace CaptchaSharp.Models.AntiCaptcha.Responses.Solutions
{
    internal class ImageCaptchaAntiCaptchaTaskSolution : AntiCaptchaTaskSolution
    {
        public string Text { get; set; }
        public string Url { get; set; }

        public override CaptchaResponse ToCaptchaResponse(string id)
        {
            return new StringResponse
            {
                Id = id,
                Response = Text
            };
        }
    }
}
