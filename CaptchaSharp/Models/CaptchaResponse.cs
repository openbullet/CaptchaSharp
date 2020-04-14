namespace CaptchaSharp.Models
{
    /// <summary>A generic captcha response.</summary>
    public class CaptchaResponse
    {
        /// <summary>The captcha id which is needed to report the solution as bad.</summary>
        public long Id { get; set; }
    }
}
