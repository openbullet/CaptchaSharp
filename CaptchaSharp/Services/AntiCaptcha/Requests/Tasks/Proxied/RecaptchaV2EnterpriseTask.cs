namespace CaptchaSharp.Services.AntiCaptcha.Requests.Tasks.Proxied
{
    internal class RecaptchaV2EnterpriseTask : AntiCaptchaTask
    {
        public string WebsiteURL { get; set; }
        public string WebsiteKey { get; set; }
        public string EnterprisePayload { get; set; }

        public RecaptchaV2EnterpriseTask()
        {
            Type = "RecaptchaV2EnterpriseTask";
        }
    }
}
