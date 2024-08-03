using System.Collections.Generic;

namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;

internal class RecaptchaV2EnterpriseTaskProxyless : AntiCaptchaTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    public Dictionary<string, string>? EnterprisePayload { get; set; }

    public RecaptchaV2EnterpriseTaskProxyless()
    {
        Type = "RecaptchaV2EnterpriseTaskProxyless";
    }
}
