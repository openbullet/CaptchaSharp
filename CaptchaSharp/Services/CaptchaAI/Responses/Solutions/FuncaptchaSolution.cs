using CaptchaSharp.Models;

namespace CaptchaSharp.Services.CaptchaAI.Responses.Solutions
{
    internal class FuncaptchaSolution : Solution
    {
        public string Token { get; set; }

        public override CaptchaResponse ToCaptchaResponse(string id)
        {
            return new StringResponse
            {
                IdString = id,
                Response = Token
            };
        }
    }
}
