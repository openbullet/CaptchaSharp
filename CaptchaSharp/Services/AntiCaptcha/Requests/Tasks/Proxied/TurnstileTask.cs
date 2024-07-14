namespace CaptchaSharp.Services.AntiCaptcha.Requests.Tasks.Proxied;

internal class TurnstileTask : AntiCaptchaTask
{
    public string WebsiteKey { get; set; }
    public string WebsiteURL { get; set; }
    public string Action { get; set; }
    public string TurnstileCData { get; set; }

    public TurnstileTask()
    {
        Type = "TurnstileTask";
    }
}
