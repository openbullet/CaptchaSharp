using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks;

internal class RecaptchaV3TaskProxyless : SolveCaptchaTaskProxyless
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("page_action", NullValueHandling = NullValueHandling.Ignore)]
    public string? PageAction { get; set; }
    
    [JsonProperty("min_score", NullValueHandling = NullValueHandling.Ignore)]
    public double? MinScore { get; set; }
    
    public RecaptchaV3TaskProxyless()
    {
        Method = "RecaptchaV3TaskProxyless";
    }
}
