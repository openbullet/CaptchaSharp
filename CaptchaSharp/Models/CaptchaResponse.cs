using System;

namespace CaptchaSharp.Models
{
    /// <summary>A generic captcha response.</summary>
    public class CaptchaResponse
    {
        private string id = "0";

        /// <summary>The captcha id which is needed to report the solution as bad.</summary>
        public long Id
        {
            get => long.Parse(id);
            set => id = value.ToString();
        }

        /// <summary>
        /// The captcha id which is needed to report the solution, if it's
        /// a string instead of a long int.
        /// </summary>
        public string IdString
        {
            get => id;
            set => id = value;
        }

        /// <summary>The time when the solution was received.</summary>
        public DateTime Time { get; set; } = DateTime.Now;
    }
}
