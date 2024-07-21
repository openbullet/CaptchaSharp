using System.Collections.Generic;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaSolveRecaptchaV3Request : NopechaSolveTokenRequest
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("url")]
    public required string Url { get; set; }
    
    [JsonProperty("enterprise")]
    public bool Enterprise { get; set; }
    
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? DataS { get; set; } = new();
    
    public NopechaSolveRecaptchaV3Request()
    {
        Type = "recaptcha3";
    }
}
