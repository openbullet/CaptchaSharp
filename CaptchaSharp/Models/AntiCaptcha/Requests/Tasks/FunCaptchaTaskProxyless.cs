namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks
{
    internal class FunCaptchaTaskProxyless : AntiCaptchaTaskProxyless
    {
        public string WebsiteURL { get; set; }
        public string WebsitePublicKey { get; set; }
        public string FuncaptchaApiJSSubdomain { get; set; }

        public FunCaptchaTaskProxyless()
        {
            Type = "FunCaptchaTaskProxyless";
        }
    }
}
