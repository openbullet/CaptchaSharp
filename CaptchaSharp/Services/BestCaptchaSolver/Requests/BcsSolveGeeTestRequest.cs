using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Requests;

internal class BcsSolveGeeTestRequest : BcsSolveRequest
{
    [JsonProperty("domain")]
    public required string Domain { get; set; }
    
    [JsonProperty("gt")]
    public required string Gt { get; set; }
    
    [JsonProperty("challenge")]
    public required string Challenge { get; set; }
    
    [JsonProperty("api_server", NullValueHandling = NullValueHandling.Ignore)]
    public string? ApiServer { get; set; }
}
