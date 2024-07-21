namespace CaptchaSharp.Models.AntiCaptcha.Requests;

internal class ReportIncorrectCaptchaRequest : Request
{
    public long TaskId { get; set; }
}