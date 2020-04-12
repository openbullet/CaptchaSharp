namespace CaptchaSharp.Services.AntiCaptcha.Requests.Tasks
{
    internal class NoCaptchaTaskProxyless : AntiCaptchaTaskProxyless
    {
        public string WebsiteURL { get; set; }
        public string WebsiteKey { get; set; }
        public bool IsInvisible { get; set; }

        public NoCaptchaTaskProxyless()
        {
            Type = "NoCaptchaTaskProxyless";
        }
    }
}
