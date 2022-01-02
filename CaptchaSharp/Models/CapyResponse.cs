namespace CaptchaSharp.Models
{
    /// <summary>The solution of a Capy captcha.</summary>
    public class CapyResponse : CaptchaResponse
    {
        /// <summary></summary>
        public string CaptchaKey { get; set; }

        /// <summary></summary>
        public string ChallengeKey { get; set; }

        /// <summary></summary>
        public string Answer { get; set; }
    }
}
