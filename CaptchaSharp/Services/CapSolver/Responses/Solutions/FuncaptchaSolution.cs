using CaptchaSharp.Models;

namespace CaptchaSharp.Services.CapSolver.Responses.Solutions
{
    internal class FuncaptchaSolution : Solution
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
