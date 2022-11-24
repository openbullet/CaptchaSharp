using CaptchaSharp.Services.CaptchaAI.Requests.Tasks;

namespace CaptchaSharp.Services.CaptchaAI.Requests
{
    internal class CaptchaTaskRequest : Request
    {
        public CaptchaAITaskProxyless Task { get; set; }
        public string AppId { get; set; } = "";
        public string LanguagePool { get; set; } = "en";
    }
}
