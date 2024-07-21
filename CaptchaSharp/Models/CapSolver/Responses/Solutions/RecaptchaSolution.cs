using CaptchaSharp.Models;

namespace CaptchaSharp.Models.CapSolver.Responses.Solutions
{
    internal class RecaptchaSolution : Solution
    {
        public string GRecaptchaResponse { get; set; }

        public override CaptchaResponse ToCaptchaResponse(string id)
        {
            return new StringResponse
            {
                Id = id,
                Response = GRecaptchaResponse
            };
        }
    }
}
