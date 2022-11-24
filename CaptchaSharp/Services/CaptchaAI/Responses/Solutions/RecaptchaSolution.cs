using CaptchaSharp.Models;

namespace CaptchaSharp.Services.CaptchaAI.Responses.Solutions
{
    internal class RecaptchaSolution : Solution
    {
        public string GRecaptchaResponse { get; set; }

        public override CaptchaResponse ToCaptchaResponse(string id)
        {
            return new StringResponse
            {
                IdString = id,
                Response = GRecaptchaResponse
            };
        }
    }
}
