using Newtonsoft.Json;

namespace CaptchaSharp.Models.AntiCaptcha.Responses.Solutions;

internal class GeeTestAntiCaptchaTaskSolution : AntiCaptchaTaskSolution
{
    [JsonProperty("challenge")]
    public string? Challenge { get; set; }
    
    [JsonProperty("validate")]
    public string? Validate { get; set; }
    
    [JsonProperty("seccode")]
    public string? SecCode { get; set; }

    public override CaptchaResponse ToCaptchaResponse(string id)
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
