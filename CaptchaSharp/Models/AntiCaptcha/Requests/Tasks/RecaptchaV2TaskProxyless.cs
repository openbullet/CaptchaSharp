namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;

internal class RecaptchaV2TaskProxyless : AntiCaptchaTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    public bool IsInvisible { get; set; }

    public RecaptchaV2TaskProxyless()
    {
        Type = "RecaptchaV2TaskProxyless";
    }
}
