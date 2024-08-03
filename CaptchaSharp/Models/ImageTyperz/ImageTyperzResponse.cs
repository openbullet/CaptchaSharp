namespace CaptchaSharp.Models.ImageTyperz;

/// <summary>
/// The response for a captcha task.
/// </summary>
public class ImageTyperzResponse
{
    /// <summary>
    /// The captcha id.
    /// </summary>
    public required long CaptchaId { get; set; }

    /// <summary>
    /// The response (if solved).
    /// </summary>
    public required string Response { get; set; }

    /// <summary>
    /// The status of the task.
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// The error message (if any).
    /// </summary>
    public required string Error { get; set; }
}
