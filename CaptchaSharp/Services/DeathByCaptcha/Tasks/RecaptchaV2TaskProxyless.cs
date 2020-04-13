namespace CaptchaSharp.Services.DeathByCaptcha.Tasks
{
    internal class RecaptchaV2TaskProxyless : DBCTaskProxyless
    {
        public string GoogleKey { get; set; }
        public string PageUrl { get; set; }
    }
}
