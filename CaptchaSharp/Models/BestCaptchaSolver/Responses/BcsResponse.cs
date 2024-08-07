using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsResponse
{
    [JsonProperty("status")]
    public required string Status { get; set; }
    
    [JsonProperty("error")]
    public string? Error { get; set; }
    
    [JsonIgnore]
    public bool Success => Status != "error";
}
