namespace CaptchaSharp.Services.CaptchaAI.Requests.Tasks.Proxied
{
    internal class HCaptchaTask : CaptchaAITask
    {
        public string WebsiteKey { get; set; }
        public string WebsiteURL { get; set; }

        public HCaptchaTask()
        {
            Type = "HCaptchaTask";
        }
    }
}
