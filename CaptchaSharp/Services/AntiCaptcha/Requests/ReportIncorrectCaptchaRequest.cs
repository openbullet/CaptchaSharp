namespace CaptchaSharp.Services.AntiCaptcha.Requests
{
    internal class ReportIncorrectCaptchaRequest : Request
    {
        public int TaskId { get; set; }
    }
}
