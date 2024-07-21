namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;

internal class HCaptchaTaskProxyless : AntiCaptchaTaskProxyless
{
    public string WebsiteKey { get; set; }
    public string WebsiteURL { get; set; }

    public HCaptchaTaskProxyless()
    {
        Type = "HCaptchaTaskProxyless";
    }
}