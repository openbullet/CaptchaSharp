using CaptchaSharp.Models;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.TwoCaptcha;

internal class TwoCaptchaGeeTestResponse : TwoCaptchaResponse
{
    public new GeeTestSolution? Request { get; set; }
}

internal class GeeTestSolution
{
    [JsonProperty("geetest_challenge")]
    public string? Challenge { get; set; }

    [JsonProperty("geetest_validate")]
    public string? Validate { get; set; }

    [JsonProperty("geetest_seccode")]
    public string? SecCode { get; set; }

    public GeeTestResponse ToGeeTestResponse(string id)
    {
        return new GeeTestResponse
        {
            Id = id,
            Challenge = Challenge!,
            Validate = Validate!,
            SecCode = SecCode!
        };
    }
}
