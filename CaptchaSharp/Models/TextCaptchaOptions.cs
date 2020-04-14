using CaptchaSharp.Enums;

namespace CaptchaSharp.Models
{
    /// <summary>Provides additional options for a text based captcha task.</summary>
    public class TextCaptchaOptions
    {
        /// <summary>The language group of the text.</summary>
        public CaptchaLanguageGroup CaptchaLanguageGroup { get; set; } = CaptchaLanguageGroup.NotSpecified;

        /// <summary>The language of the text.</summary>
        public CaptchaLanguage CaptchaLanguage { get; set; } = CaptchaLanguage.NotSpecified;
    }
}
