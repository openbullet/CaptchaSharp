namespace CaptchaSharp.Services.AntiCaptcha.Requests.Tasks
{
    internal class RecaptchaV3Task : AntiCaptchaTask
    {
        public string WebsiteURL { get; set; }
        public string WebsiteKey { get; set; }
        public string PageAction { get; set; }
        public float MinScore { get; set; }

        public RecaptchaV3Task()
        {
            Type = "RecaptchaV3Task";
        }
    }
}
