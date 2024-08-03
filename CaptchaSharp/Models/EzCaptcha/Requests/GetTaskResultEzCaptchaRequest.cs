namespace CaptchaSharp.Models.EzCaptcha.Requests;

internal class GetTaskResultEzCaptchaRequest : EzCaptchaRequest
{
    public required string TaskId { get; set; }
}
