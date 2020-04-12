namespace CaptchaSharp.Services.AntiCaptcha.Requests
{
    internal class CaptchaTaskRequest : Request
    {
        public string Task { get; set; }
        public int SoftId { get; set; } = 0;
        public string LanguagePool { get; set; } = "en";
    }
}
