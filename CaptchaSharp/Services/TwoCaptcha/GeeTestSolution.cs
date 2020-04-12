using CaptchaSharp.Models;

namespace CaptchaSharp.Services.TwoCaptcha
{
    internal class GeeTestSolution
    {
        public string Challenge { get; set; }
        public string Validate { get; set; }
        public string SecCode { get; set; }

        public GeeTestResponse ToGeeTestResponse(long id)
        {
            return new GeeTestResponse()
            {
                Id = id,
                Challenge = Challenge,
                Validate = Validate,
                SecCode = SecCode
            };
        }
    }
}
