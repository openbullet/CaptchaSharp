using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsSolveGeeTestResponse
{
    [JsonProperty("solution")]
    public GeeTestSolution? Solution { get; set; }
}

internal class GeeTestSolution
{
    [JsonProperty("challenge")]
    public required string Challenge { get; set; }
    
    [JsonProperty("validate")]
    public required string Validate { get; set; }
    
    [JsonProperty("seccode")]
    public required string SecCode { get; set; }
}
