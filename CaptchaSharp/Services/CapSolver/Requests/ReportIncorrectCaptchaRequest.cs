namespace CaptchaSharp.Services.CapSolver.Requests
{
    internal class ReportIncorrectCaptchaRequest : Request
    {
        public string TaskId { get; set; }
    }
}
