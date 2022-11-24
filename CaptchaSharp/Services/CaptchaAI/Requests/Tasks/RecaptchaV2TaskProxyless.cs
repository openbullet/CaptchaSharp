namespace CaptchaSharp.Services.CaptchaAI.Requests.Tasks
{
    internal class RecaptchaV2TaskProxyless : CaptchaAITaskProxyless
    {
        public string WebsiteURL { get; set; }
        public string WebsiteKey { get; set; }
        public bool IsInvisible { get; set; }
        public string RecaptchaDataSValue { get; set; } = string.Empty;

        public RecaptchaV2TaskProxyless()
        {
            Type = "RecaptchaV2TaskProxyless";
        }
    }
}
