using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks.Proxied;

internal class RecaptchaV2Task : SolveCaptchaTask
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("data_s", NullValueHandling = NullValueHandling.Ignore)]
    public string? DataS { get; set; }
    
    public RecaptchaV2Task()
    {
        Method = "RecaptchaV2Task";
    }
}
