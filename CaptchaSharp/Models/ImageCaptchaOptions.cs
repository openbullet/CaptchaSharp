using CaptchaSharp.Enums;

namespace CaptchaSharp.Models
{
    public class ImageCaptchaOptions
    {
        public bool IsPhrase { get; set; } = false;

        public bool CaseSensitive { get; set; } = false;

        public CharacterSet CharactersType { get; set; } = CharacterSet.NotSpecified;

        public bool RequiresCalculation { get; set; } = false;

        public int MinLength { get; set; } = 0;

        public int MaxLength { get; set; } = 0;

        public CaptchaLanguageGroup LanguageGroup = CaptchaLanguageGroup.NotSpecified;

        public CaptchaLanguage Language = CaptchaLanguage.NotSpecified;

        public string TextInstructions = "";
    }
}
