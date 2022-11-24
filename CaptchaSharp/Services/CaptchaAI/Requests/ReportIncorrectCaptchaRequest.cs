namespace CaptchaSharp.Services.CaptchaAI.Requests
{
    internal class ReportIncorrectCaptchaRequest : Request
    {
        public string TaskId { get; set; }
    }
}
