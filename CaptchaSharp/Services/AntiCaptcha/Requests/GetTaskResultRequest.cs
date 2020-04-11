namespace CaptchaSharp.Services.AntiCaptcha.Requests
{
    internal class GetTaskResultRequest : Request
    {
        public int TaskId { get; set; }
    }
}
