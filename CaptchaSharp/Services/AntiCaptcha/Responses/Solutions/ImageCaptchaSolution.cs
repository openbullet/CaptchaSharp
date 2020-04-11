using CaptchaSharp.Models;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions
{
    internal class ImageCaptchaSolution : ITaskSolution
    {
        public string Text { get; set; }
        public string Url { get; set; }

        public CaptchaResponse ToCaptchaResponse(int id)
        {
            return new StringResponse
            {
                Id = id,
                Response = Text
            };
        }
    }
}
