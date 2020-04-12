namespace CaptchaSharp.Services.AntiCaptcha.Requests.Tasks
{
    internal class NoCaptchaTask : AntiCaptchaTask
    {
        public string WebsiteURL { get; set; }
        public string WebsiteKey { get; set; }
        public bool IsInvisible { get; set; }

        public NoCaptchaTask()
        {
            Type = "NoCaptchaTask";
        }
    }
}
