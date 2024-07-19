using System.Collections.Generic;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.Nopecha;

internal class NopechaSolveCloudflareTurnstileRequest : NopechaSolveTokenRequest
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("url")]
    public required string Url { get; set; }
    
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, object>? Data { get; set; } = new();

    public NopechaSolveCloudflareTurnstileRequest()
    {
        Type = "turnstile";
    }
}
