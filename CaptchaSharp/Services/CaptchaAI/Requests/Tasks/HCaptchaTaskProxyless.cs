namespace CaptchaSharp.Services.CaptchaAI.Requests.Tasks
{
    internal class HCaptchaTaskProxyless : CaptchaAITaskProxyless
    {
        public string WebsiteKey { get; set; }
        public string WebsiteURL { get; set; }

        public HCaptchaTaskProxyless()
        {
            Type = "HCaptchaTaskProxyless";
        }
    }
}
