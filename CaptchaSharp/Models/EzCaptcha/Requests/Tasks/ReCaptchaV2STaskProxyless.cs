namespace CaptchaSharp.Models.EzCaptcha.Requests.Tasks;

internal class ReCaptchaV2STaskProxyless : EzCaptchaTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    public bool IsInvisible { get; set; }
    public string? DataS { get; set; }
    
    public ReCaptchaV2STaskProxyless()
    {
        Type = "ReCaptchaV2STaskProxyless";
    }
}
