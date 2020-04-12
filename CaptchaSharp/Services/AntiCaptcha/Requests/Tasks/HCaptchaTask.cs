namespace CaptchaSharp.Services.AntiCaptcha.Requests.Tasks
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
