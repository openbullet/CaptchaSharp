namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks.Proxied;

internal class RecaptchaV2Task : AntiCaptchaTask
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    public bool IsInvisible { get; set; }

    public RecaptchaV2Task()
    {
        Type = "RecaptchaV2Task";
    }
}
