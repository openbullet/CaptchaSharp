using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;

internal class HCaptchaTaskProxyless : AntiCaptchaTaskProxyless
{
    [JsonProperty("websiteKey")]
    public required string WebsiteKey { get; set; }
    
    [JsonProperty("websiteURL")]
    public required string WebsiteUrl { get; set; }
    
    [JsonProperty("isInvisible")]
    public bool IsInvisible { get; set; }
    
    [JsonProperty("isEnterprise")]
    public bool IsEnterprise { get; set; }
    
    [JsonProperty("enterprisePayload", NullValueHandling = NullValueHandling.Ignore)]
    public JObject? EnterprisePayload { get; set; }

    public HCaptchaTaskProxyless()
    {
        Type = "HCaptchaTaskProxyless";
    }
}
