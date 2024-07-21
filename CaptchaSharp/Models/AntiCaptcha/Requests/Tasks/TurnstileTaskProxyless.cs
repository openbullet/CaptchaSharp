namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;

internal class TurnstileTaskProxyless : AntiCaptchaTaskProxyless
{
    public string WebsiteKey { get; set; }
    public string WebsiteURL { get; set; }
    public string Action { get; set; }
    public string TurnstileCData { get; set; }

    public TurnstileTaskProxyless()
    {
        Type = "TurnstileTaskProxyless";
    }
}
