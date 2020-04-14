using CaptchaSharp.Enums;

namespace CaptchaSharp.Models
{
    /// <summary>Provides additional options for an image based captcha task.</summary>
    public class ImageCaptchaOptions
    {
        /// <summary>Whether the captcha is made of multiple words.</summary>
        public bool IsPhrase { get; set; } = false;

        /// <summary>Whether the captcha should be solved as case sensitive.</summary>
        public bool CaseSensitive { get; set; } = true;

        /// <summary>The set of allowed characters.</summary>
        public CharacterSet CharacterSet { get; set; } = CharacterSet.NotSpecified;

        /// <summary>Whether the captcha includes mathematical calculations.</summary>
        public bool RequiresCalculation { get; set; } = false;

        /// <summary>The minimum possible length of the text.</summary>
        public int MinLength { get; set; } = 0;

        /// <summary>The maximum possible length of the text.</summary>
        public int MaxLength { get; set; } = 0;

        /// <summary>The language group of the text.</summary>
        public CaptchaLanguageGroup CaptchaLanguageGroup { get; set; } = CaptchaLanguageGroup.NotSpecified;

        /// <summary>The language of the text.</summary>
        public CaptchaLanguage CaptchaLanguage { get; set; } = CaptchaLanguage.NotSpecified;

        /// <summary>Any additional text instruction (e.g. type the characters in red).</summary>
        public string TextInstructions { get; set; } = "";
    }
}
