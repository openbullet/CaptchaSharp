namespace CaptchaSharp.Models.EzCaptcha.Requests.Tasks;

internal class ReCaptchaV2EnterpriseTaskProxyless : EzCaptchaTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    public bool IsInvisible { get; set; }
    
    public ReCaptchaV2EnterpriseTaskProxyless()
    {
        Type = "ReCaptchaV2EnterpriseTaskProxyless";
    }
}
