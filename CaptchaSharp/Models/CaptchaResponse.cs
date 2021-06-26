using System;

namespace CaptchaSharp.Models
{
    /// <summary>A generic captcha response.</summary>
    public class CaptchaResponse
    {
        /// <summary>The captcha id which is needed to report the solution as bad.</summary>
        public long Id { get; set; }

        /// <summary>The time when the solution was received.</summary>
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
