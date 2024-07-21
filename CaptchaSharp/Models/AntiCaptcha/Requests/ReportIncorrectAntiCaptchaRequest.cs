namespace CaptchaSharp.Models.AntiCaptcha.Requests;

internal class ReportIncorrectAntiCaptchaRequest : AntiCaptchaRequest
{
    public long TaskId { get; set; }
}
