namespace CaptchaSharp.Models.EzCaptcha.Requests.Tasks;

internal class HcaptchaTaskProxyless : EzCaptchaTaskProxyless
{
    public string? WebsiteKey { get; set; }
    public string? WebsiteURL { get; set; }
    
    public HcaptchaTaskProxyless()
    {
        Type = "HcaptchaTaskProxyless";
    }
}
