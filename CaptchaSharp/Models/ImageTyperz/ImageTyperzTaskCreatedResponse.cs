namespace CaptchaSharp.Models.ImageTyperz;

/// <summary>
/// The response for a captcha task after it's created.
/// </summary>
public class ImageTyperzTaskCreatedResponse
{
    /// <summary>
    /// The captcha id.
    /// </summary>
    public required long CaptchaId { get; set; }
}
