namespace CaptchaSharp.Services.CaptchaAI.Requests.Tasks
{
    internal class FunCaptchaTaskProxyless : CaptchaAITaskProxyless
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
