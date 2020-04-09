using CaptchaSharp.Enums;

namespace CaptchaSharp.Models
{
    public class TextCaptchaOptions
    {
        public CaptchaLanguageGroup LanguageGroup { get; set; } = CaptchaLanguageGroup.NotSpecified;

        public CaptchaLanguage Language { get; set; } = CaptchaLanguage.NotSpecified;
    }
}
