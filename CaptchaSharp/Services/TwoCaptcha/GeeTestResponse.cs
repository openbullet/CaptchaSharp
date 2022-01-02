using CaptchaSharp.Models;

namespace CaptchaSharp.Services.TwoCaptcha
{
    internal class TwoCaptchaGeeTestResponse : Response
    {
        public new GeeTestSolution Request { get; set; }
    }

    internal class GeeTestSolution
    {
        public string Challenge { get; set; }
        public string Validate { get; set; }
        public string Seccode { get; set; }

        public GeeTestResponse ToGeeTestResponse(long id)
        {
            return new GeeTestResponse()
            {
                Id = id,
                Challenge = Challenge,
                Validate = Validate,
                SecCode = Seccode
            };
        }
    }
}
