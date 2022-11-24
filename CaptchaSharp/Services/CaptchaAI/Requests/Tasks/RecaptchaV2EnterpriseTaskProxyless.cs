namespace CaptchaSharp.Services.CaptchaAI.Requests.Tasks
{
    internal class RecaptchaV2EnterpriseTaskProxyless : CaptchaAITaskProxyless
    {
        public string WebsiteURL { get; set; }
        public string WebsiteKey { get; set; }
        public string EnterprisePayload { get; set; }

        public RecaptchaV2EnterpriseTaskProxyless()
        {
            Type = "RecaptchaV2EnterpriseTaskProxyless";
        }
    }
}
