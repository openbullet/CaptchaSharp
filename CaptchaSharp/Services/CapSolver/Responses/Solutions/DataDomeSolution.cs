using CaptchaSharp.Models;

namespace CaptchaSharp.Services.CapSolver.Responses.Solutions
{
    internal class DataDomeSolution : Solution
    {
        public string Cookie { get; set; }

        public override CaptchaResponse ToCaptchaResponse(string id)
        {
            return new StringResponse
            {
                IdString = id,
                Response = Cookie
            };
        }
    }
}
