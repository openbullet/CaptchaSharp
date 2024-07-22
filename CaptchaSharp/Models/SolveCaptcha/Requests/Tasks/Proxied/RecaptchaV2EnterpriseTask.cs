using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks.Proxied;

internal class RecaptchaV2EnterpriseTask : SolveCaptchaTask
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("enterprisePayload", NullValueHandling = NullValueHandling.Ignore)]
    public string? EnterprisePayload { get; set; }

    public RecaptchaV2EnterpriseTask()
    {
        Method = "RecaptchaV2EnterpriseTask";
    }
}
