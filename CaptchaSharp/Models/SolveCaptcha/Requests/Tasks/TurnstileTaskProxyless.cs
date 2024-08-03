using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks;

internal class TurnstileTaskProxyless : SolveCaptchaTaskProxyless
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
    
    public TurnstileTaskProxyless()
    {
        Method = "TurnstileTaskProxyless";
    }
}
