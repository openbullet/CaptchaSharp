namespace CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied
{
    internal class RecaptchaV3Task : DBCTask
    {
        public string GoogleKey { get; set; }
        public string PageUrl { get; set; }
        public string Action { get; set; }
        public float Min_Score { get; set; } = 0.3F;
    }
}
