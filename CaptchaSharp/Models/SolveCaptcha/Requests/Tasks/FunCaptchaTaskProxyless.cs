using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks;

internal class FunCaptchaTaskProxyless : SolveCaptchaTaskProxyless
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("site_key")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("sub_domain", NullValueHandling = NullValueHandling.Ignore)]
    public string? SubDomain { get; set; }
    
    [JsonProperty("data_s", NullValueHandling = NullValueHandling.Ignore)]
    public string? Data { get; set; }
    
    public FunCaptchaTaskProxyless()
    {
        Method = "FunCaptchaTaskProxyless";
    }
}
