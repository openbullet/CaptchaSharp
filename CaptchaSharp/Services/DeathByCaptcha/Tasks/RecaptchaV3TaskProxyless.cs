namespace CaptchaSharp.Services.DeathByCaptcha.Tasks
{
    internal class RecaptchaV3TaskProxyless : DBCTaskProxyless
    {
        public string GoogleKey { get; set; }
        public string PageUrl { get; set; }
        public string Action { get; set; }
        public float Min_Score { get; set; } = 0.3F;
    }
}
