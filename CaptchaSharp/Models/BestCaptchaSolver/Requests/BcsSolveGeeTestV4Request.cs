using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Requests;

internal class BcsSolveGeeTestV4Request : BcsSolveRequest
{
    [JsonProperty("domain")]
    public required string Domain { get; set; }
    
    [JsonProperty("captchaid")]
    public required string CaptchaId { get; set; }
}
