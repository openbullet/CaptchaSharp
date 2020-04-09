using CaptchaSharp.Enums;

namespace CaptchaSharp.Models
{
    public class TextCaptchaOptions
    {
        public CaptchaLanguageGroup CaptchaLanguageGroup { get; set; } = CaptchaLanguageGroup.NotSpecified;

        public CaptchaLanguage CaptchaLanguage { get; set; } = CaptchaLanguage.NotSpecified;
    }
}
