namespace CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied
{
    internal class RecaptchaV2Task : DBCTask
    {
        public string GoogleKey { get; set; }
        public string PageUrl { get; set; }
    }
}
