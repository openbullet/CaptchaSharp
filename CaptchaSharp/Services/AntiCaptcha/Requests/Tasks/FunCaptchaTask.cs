namespace CaptchaSharp.Services.AntiCaptcha.Requests.Tasks
{
    internal class FunCaptchaTask : AntiCaptchaTask
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
