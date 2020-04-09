using CaptchaSharp.Enums;

namespace CaptchaSharp.Models
{
    public class ImageCaptchaOptions
    {
        public bool IsPhrase { get; set; } = false;

        public bool CaseSensitive { get; set; } = true;

        public CharacterSet CharacterSet { get; set; } = CharacterSet.NotSpecified;

        public bool RequiresCalculation { get; set; } = false;

        public int MinLength { get; set; } = 0;

        public int MaxLength { get; set; } = 0;

        public CaptchaLanguageGroup CaptchaLanguageGroup { get; set; } = CaptchaLanguageGroup.NotSpecified;

        public CaptchaLanguage CaptchaLanguage { get; set; } = CaptchaLanguage.NotSpecified;

        public string TextInstructions { get; set; } = "";
    }
}
