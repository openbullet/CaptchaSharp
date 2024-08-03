namespace CaptchaSharp.Models.AntiCaptcha.Requests;

internal class GetTaskResultAntiCaptchaRequest : AntiCaptchaRequest
{
    public int TaskId { get; set; }
}