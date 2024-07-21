using Newtonsoft.Json;

namespace CaptchaSharp.Models.EzCaptcha.Requests.Tasks;

internal class ReCaptchaV3TaskProxyless : EzCaptchaTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    
    [JsonProperty("pageAction", NullValueHandling = NullValueHandling.Ignore)]
    public string? PageAction { get; set; }

    public ReCaptchaV3TaskProxyless()
    {
        Type = "ReCaptchaV3TaskProxyless";
    }
}
