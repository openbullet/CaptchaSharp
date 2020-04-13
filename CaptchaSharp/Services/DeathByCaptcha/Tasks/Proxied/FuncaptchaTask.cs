namespace CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied
{
    internal class FuncaptchaTask : DBCTask
    {
        public string PublicKey { get; set; }
        public string PageUrl { get; set; }
    }
}
