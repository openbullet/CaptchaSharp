namespace CaptchaSharp.Models;

/// <summary>
/// A captcha response for Cloudflare's Turnstile.
/// </summary>
public class CloudflareTurnstileResponse : CaptchaResponse
{
    /// <summary>
    /// The response token.
    /// </summary>
    public required string Response { get; set; }

    /// <summary>
    /// The user agent used to solve the challenge. It must be also used
    /// when submitting the response to the target website.
    /// </summary>
    public required string UserAgent { get; set; }
}
