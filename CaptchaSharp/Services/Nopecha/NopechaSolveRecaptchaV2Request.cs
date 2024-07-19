using System.Collections.Generic;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.Nopecha;

internal class NopechaSolveRecaptchaV2Request : NopechaSolveTokenRequest
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("url")]
    public required string Url { get; set; }
    
    [JsonProperty("enterprise")]
    public bool Enterprise { get; set; }
    
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? DataS { get; set; } = new();

    public NopechaSolveRecaptchaV2Request()
    {
        Type = "recaptcha2";
    }
}
