namespace CaptchaSharp.Models.AntiCaptcha.Responses;

/// <summary>
/// Represents the response from the AntiCaptcha API when creating a task.
/// </summary>
public class TaskCreationAntiCaptchaResponse : AntiCaptchaResponse
{
    /// <summary>
    /// The ID of the created task.
    /// </summary>
    public int TaskId { get; set; }
}
