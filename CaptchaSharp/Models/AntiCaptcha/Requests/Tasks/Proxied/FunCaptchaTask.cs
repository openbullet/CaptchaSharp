using Newtonsoft.Json;

namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks.Proxied;

internal class FunCaptchaTask : AntiCaptchaTask
{
    [JsonProperty("websiteURL")]
    public required string WebsiteUrl { get; set; }
    
    [JsonProperty("websitePublicKey")]
    public required string WebsitePublicKey { get; set; }
    
    [JsonProperty("funcaptchaApiJSSubdomain", NullValueHandling = NullValueHandling.Ignore)]
    public string? FuncaptchaApiJsSubdomain { get; set; }
    
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public string? Data { get; set; }

    public FunCaptchaTask()
    {
        Type = "FunCaptchaTask";
    }
}
