using CaptchaSharp.Models;

namespace CaptchaSharp.Services.CaptchaAI.Responses.Solutions
{
    internal class ImageCaptchaSolution : Solution
    {
        public string Text { get; set; }
        public string Url { get; set; }

        public override CaptchaResponse ToCaptchaResponse(string id)
        {
            return new StringResponse
            {
                IdString = id,
                Response = Text
            };
        }
    }
}
