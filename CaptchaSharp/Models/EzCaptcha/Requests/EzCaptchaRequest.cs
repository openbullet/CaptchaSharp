namespace CaptchaSharp.Models.EzCaptcha.Requests;

/// <summary>
/// Represents a request to the EzCaptcha API.
/// </summary>
public class EzCaptchaRequest
{
    /// <summary>
    /// Your EzCaptcha API key.
    /// </summary>
    public string ClientKey { get; set; } = "";
}
