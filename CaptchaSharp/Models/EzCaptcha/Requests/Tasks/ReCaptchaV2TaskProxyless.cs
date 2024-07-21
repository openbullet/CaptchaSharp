namespace CaptchaSharp.Models.EzCaptcha.Requests.Tasks;

internal class ReCaptchaV2TaskProxyless : EzCaptchaTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    public bool IsInvisible { get; set; }

    public ReCaptchaV2TaskProxyless()
    {
        Type = "ReCaptchaV2TaskProxyless";
    }
}
