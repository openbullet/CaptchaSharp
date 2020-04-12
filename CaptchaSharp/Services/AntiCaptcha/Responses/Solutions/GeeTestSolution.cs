using CaptchaSharp.Models;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions
{
    internal class GeeTestSolution : ITaskSolution
    {
        public string Challenge { get; set; }
        public string Validate { get; set; }
        public string SecCode { get; set; }

        public CaptchaResponse ToCaptchaResponse(long id)
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
