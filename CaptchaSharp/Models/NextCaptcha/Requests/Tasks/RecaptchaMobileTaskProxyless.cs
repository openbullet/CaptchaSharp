using CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;

namespace CaptchaSharp.Models.NextCaptcha.Requests.Tasks;

internal class RecaptchaMobileTaskProxyless : AntiCaptchaTaskProxyless
{
    public required string AppPackageName { get; set; }
    public required string AppKey { get; set; }
    public required string AppAction { get; set; }
    
    public RecaptchaMobileTaskProxyless()
    {
        Type = "RecaptchaMobileTaskProxyless";
    }
}
