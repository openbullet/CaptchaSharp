namespace CaptchaSharp.Services.AntiCaptcha.Requests
{
    internal class ReportIncorrectCaptchaRequest : Request
    {
        public long TaskId { get; set; }
    }
}
