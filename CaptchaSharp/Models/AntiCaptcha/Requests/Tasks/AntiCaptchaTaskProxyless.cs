namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;

/// <summary>
/// A task that does not require a proxy.
/// </summary>
public class AntiCaptchaTaskProxyless
{
    /// <summary>
    /// The type of the task.
    /// </summary>
    public string? Type { get; set; }
}
