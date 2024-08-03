using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks.Proxied;

internal class HCaptchaTask : SolveCaptchaTask
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("isInvisible")]
    public bool IsInvisible { get; set; }
    
    [JsonProperty("data_s", NullValueHandling = NullValueHandling.Ignore)]
    public string? DataS { get; set; }
    
    public HCaptchaTask()
    {
        Method = "HCaptchaTask";
    }
}
