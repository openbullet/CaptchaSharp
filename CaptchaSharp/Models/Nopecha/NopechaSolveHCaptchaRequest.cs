using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaSolveHCaptchaRequest : NopechaSolveTokenRequest
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("url")]
    public required string Url { get; set; }
    
    [JsonProperty("data")]
    public JObject? Data { get; set; }
    
    public NopechaSolveHCaptchaRequest()
    {
        Type = "hcaptcha";
    }
}
