namespace CaptchaSharp.Services.CaptchaAI.Requests.Tasks.Proxied
{
    internal class RecaptchaV2Task : CaptchaAITask
    {
        public string WebsiteURL { get; set; }
        public string WebsiteKey { get; set; }
        public bool IsInvisible { get; set; }
        public string RecaptchaDataSValue { get; set; } = string.Empty;

        public RecaptchaV2Task()
        {
            Type = "RecaptchaV2Task";
        }
    }
}
