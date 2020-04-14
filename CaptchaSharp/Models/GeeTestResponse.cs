namespace CaptchaSharp.Models
{
    /// <summary>The solution of a GeeTest captcha.</summary>
    public class GeeTestResponse : CaptchaResponse
    {
        /// <summary></summary>
        public string Challenge { get; set; }

        /// <summary></summary>
        public string Validate { get; set; }

        /// <summary></summary>
        public string SecCode { get; set; }
    }
}
