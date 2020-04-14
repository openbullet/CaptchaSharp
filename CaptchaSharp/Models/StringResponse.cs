namespace CaptchaSharp.Models
{
    /// <summary>A captcha response with a string solution.</summary>
    public class StringResponse : CaptchaResponse
    {
        /// <summary>The plaintext response string.</summary>
        public string Response { get; set; }
    }
}
