using CaptchaSharp.Models;

namespace CaptchaSharp.Services.TwoCaptcha
{
    internal class GeeTestSolution
    {
        public string GeeTest_Challenge { get; set; }
        public string GeeTest_Validate { get; set; }
        public string GeeTest_SecCode { get; set; }

        public GeeTestResponse ToGeeTestResponse(long id)
        {
            return new GeeTestResponse()
            {
                Id = id,
                Challenge = GeeTest_Challenge,
                Validate = GeeTest_Validate,
                SecCode = GeeTest_SecCode
            };
        }
    }
}
