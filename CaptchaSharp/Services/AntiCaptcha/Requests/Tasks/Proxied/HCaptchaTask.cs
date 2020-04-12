namespace CaptchaSharp.Services.AntiCaptcha.Requests.Tasks.Proxied
{
    internal class HCaptchaTask : AntiCaptchaTask
    {
        public string WebsiteKey { get; set; }
        public string WebsiteURL { get; set; }

        public HCaptchaTask()
        {
            Type = "HCaptchaTask";
        }
    }
}
