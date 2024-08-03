using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsSolveGeeTestV4Response
{
    [JsonProperty("solution")]
    public GeeTestV4Solution? Solution { get; set; }
}

internal class GeeTestV4Solution
{
    [JsonProperty("captcha_id")]
    public required string CaptchaId { get; set; }
    
    [JsonProperty("lot_number")]
    public required string LotNumber { get; set; }
    
    [JsonProperty("pass_token")]
    public required string PassToken { get; set; }
    
    [JsonProperty("gen_time")]
    public required string GenTime { get; set; }
    
    [JsonProperty("captcha_output")]
    public required string CaptchaOutput { get; set; }
}
