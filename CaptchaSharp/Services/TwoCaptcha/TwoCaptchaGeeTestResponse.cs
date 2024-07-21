using CaptchaSharp.Models;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.TwoCaptcha;

internal class TwoCaptchaGeeTestResponse : TwoCaptchaResponse
{
    public new GeeTestSolution Request { get; set; }
}

internal class GeeTestSolution
{
    [JsonProperty(PropertyName = "geetest_challenge")]
    public string Challenge { get; set; }

    [JsonProperty(PropertyName = "geetest_validate")]
    public string Validate { get; set; }

    [JsonProperty(PropertyName = "geetest_seccode")]
    public string Seccode { get; set; }

    public GeeTestResponse ToGeeTestResponse(string id)
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
