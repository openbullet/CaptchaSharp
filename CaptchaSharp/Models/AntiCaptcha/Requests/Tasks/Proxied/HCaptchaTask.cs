namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks.Proxied;

internal class HCaptchaTask : AntiCaptchaTask
{
    public string WebsiteKey { get; set; }
    public string WebsiteURL { get; set; }

    public HCaptchaTask()
    {
        Type = "HCaptchaTask";
    }
}