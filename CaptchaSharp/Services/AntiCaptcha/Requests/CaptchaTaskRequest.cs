using CaptchaSharp.Services.AntiCaptcha.Requests.Tasks;

namespace CaptchaSharp.Services.AntiCaptcha.Requests
{
    internal class CaptchaTaskRequest : Request
    {
        public AntiCaptchaTaskProxyless Task { get; set; }
        public int SoftId { get; set; } = 0;
        public string LanguagePool { get; set; } = "en";
    }
}
