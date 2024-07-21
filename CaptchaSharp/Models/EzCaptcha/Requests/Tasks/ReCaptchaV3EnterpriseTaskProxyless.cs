using CaptchaSharp.Models.EzCaptcha.Requests.Tasks;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.EzCaptcha.Requests.Tasks;

internal class ReCaptchaV3EnterpriseTaskProxyless : EzCaptchaTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    
    [JsonProperty("pageAction", NullValueHandling = NullValueHandling.Ignore)]
    public string? PageAction { get; set; }
    
    public ReCaptchaV3EnterpriseTaskProxyless()
    {
        Type = "ReCaptchaV3EnterpriseTaskProxyless";
    }
}
