namespace CaptchaSharp.Models.EzCaptcha.Responses;

/// <summary>
/// Represents the response from the EzCaptcha API when creating a task.
/// </summary>
public class TaskCreationEzCaptchaResponse : EzCaptchaResponse
{
    /// <summary>
    /// The ID of the created task.
    /// </summary>
    public string? TaskId { get; set; }
}
