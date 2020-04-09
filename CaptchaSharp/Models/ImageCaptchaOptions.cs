using CaptchaSharp.Enums;

namespace CaptchaSharp.Models
{
    public struct ImageCaptchaOptions
    {
        public bool IsPhrase;

        public bool CaseSensitive;

        public CharactersType CharactersType;

        public bool RequiresCalculation;

        public int MinLength;

        public int MaxLength;

        public CaptchaLanguageGroup LanguageGroup;

        public CaptchaLanguage Language;

        public string Instructions;
    }
}
