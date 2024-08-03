using CaptchaSharp.Models.AntiCaptcha.Requests.Tasks.Proxied;

namespace CaptchaSharp.Models.NextCaptcha.Requests.Tasks.Proxied;

internal class RecaptchaMobileTask : AntiCaptchaTask
{
    public required string AppPackageName { get; set; }
    public required string AppKey { get; set; }
    public required string AppAction { get; set; }
    
    public RecaptchaMobileTask()
    {
        Type = "RecaptchaMobileTask";
    }
}
