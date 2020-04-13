namespace CaptchaSharp.Services.DeathByCaptcha.Tasks
{
    internal class FuncaptchaTaskProxyless : DBCTaskProxyless
    {
        public string PublicKey { get; set; }
        public string PageUrl { get; set; }
    }
}
