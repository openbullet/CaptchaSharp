using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks.Proxied;

internal class FunCaptchaTask : SolveCaptchaTask
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("sub_domain", NullValueHandling = NullValueHandling.Ignore)]
    public string? SubDomain { get; set; }
    
    public FunCaptchaTask()
    {
        Method = "FunCaptchaTask";
    }
}
