using Newtonsoft.Json;

namespace CaptchaSharp.Services.Nopecha;

internal class NopechaSolveHCaptchaRequest : NopechaSolveTokenRequest
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("url")]
    public required string Url { get; set; }
    
    public NopechaSolveHCaptchaRequest()
    {
        Type = "hcaptcha";
    }
}
