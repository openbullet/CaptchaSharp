namespace CaptchaSharp.Services.CaptchaAI.Requests.Tasks.Proxied
{
    internal class FunCaptchaTask : CaptchaAITask
    {
        public string WebsiteURL { get; set; }
        public string WebsitePublicKey { get; set; }
        public string FuncaptchaApiJSSubdomain { get; set; }

        public FunCaptchaTask()
        {
            Type = "FunCaptchaTask";
        }
    }
}
