namespace CaptchaSharp.Services.AntiCaptcha.Requests.Tasks
{
    internal class GeeTestTaskProxyless : AntiCaptchaTaskProxyless
    {
        public string WebsiteURL { get; set; }
        public string Gt { get; set; }
        public string Challenge { get; set; }
        public string GeetestApiServerSubdomain { get; set; }

        public GeeTestTaskProxyless()
        {
            Type = "GeeTestTaskProxyless";
        }
    }
}
