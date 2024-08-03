using System.Collections.Generic;
using CaptchaSharp.Enums;

namespace CaptchaSharp.Extensions;

/// <summary>Extensions for a <see cref="CaptchaLanguage"/>.</summary>
public static class CaptchaLanguageExtensions
{
    private static readonly Dictionary<CaptchaLanguage, string> _dict = new()
    {
            [ CaptchaLanguage.NotSpecified ] = "en",
            [ CaptchaLanguage.English ] = "en",
            [ CaptchaLanguage.Russian ] = "ru",
            [ CaptchaLanguage.Spanish ] = "es",
            [ CaptchaLanguage.Portuguese ] = "pt",
            [ CaptchaLanguage.Ukrainian ] = "uk",
            [ CaptchaLanguage.Vietnamese ] = "vi",
            [ CaptchaLanguage.French ] = "fr",
            [ CaptchaLanguage.Indonesian ] = "id",
            [ CaptchaLanguage.Arab ] = "ar",
            [ CaptchaLanguage.Japanese ] = "ja",
            [ CaptchaLanguage.Turkish ] = "tr",
            [ CaptchaLanguage.German ] = "de",
            [ CaptchaLanguage.Chinese ] = "zh",
            [ CaptchaLanguage.Philippine ] = "fil",
            [ CaptchaLanguage.Polish ] = "pl",
            [ CaptchaLanguage.Thai ] = "th",
            [ CaptchaLanguage.Italian ] = "it",
            [ CaptchaLanguage.Dutch ] = "nl",
            [ CaptchaLanguage.Slovak ] = "sk",
            [ CaptchaLanguage.Bulgarian ] = "bg",
            [ CaptchaLanguage.Romanian ] = "ro",
            [ CaptchaLanguage.Hungarian ] = "hu",
            [ CaptchaLanguage.Korean ] = "ko",
            [ CaptchaLanguage.Czech ] = "cs",
            [ CaptchaLanguage.Azerbaijani ] = "az",
            [ CaptchaLanguage.Persian ] = "fa",
            [ CaptchaLanguage.Bengali ] = "bn",
            [ CaptchaLanguage.Greek ] = "el",
            [ CaptchaLanguage.Lithuanian ] = "lt",
            [ CaptchaLanguage.Latvian ] = "lv",
            [ CaptchaLanguage.Swedish ] = "sv",
            [ CaptchaLanguage.Serbian ] = "sr",
            [ CaptchaLanguage.Croatian ] = "hr",
            [ CaptchaLanguage.Hebrew ] = "he",
            [ CaptchaLanguage.Hindi ] = "hi",
            [ CaptchaLanguage.Norwegian ] = "nb",
            [ CaptchaLanguage.Slovenian ] = "sl",
            [ CaptchaLanguage.Danish ] = "da",
            [ CaptchaLanguage.Uzbek ] = "uz",
            [ CaptchaLanguage.Finnish ] = "fi",
            [ CaptchaLanguage.Catalan ] = "ca",
            [ CaptchaLanguage.Georgian ] = "ka",
            [ CaptchaLanguage.Malay ] = "ms",
            [ CaptchaLanguage.Telugu ] = "te",
            [ CaptchaLanguage.Estonian ] = "et",
            [ CaptchaLanguage.Malayalam ] = "ml",
            [ CaptchaLanguage.Belorussian ] = "be",
            [ CaptchaLanguage.Kazakh ] = "kk",
            [ CaptchaLanguage.Marathi ] = "mr",
            [ CaptchaLanguage.Nepali ] = "ne",
            [ CaptchaLanguage.Burmese ] = "my",
            [ CaptchaLanguage.Bosnian ] = "bs",
            [ CaptchaLanguage.Armenian ] = "hy",
            [ CaptchaLanguage.Macedonian ] = "mk",
            [ CaptchaLanguage.Punjabi ] = "pa"
        };

    /// <summary>Converts a <see cref="CaptchaLanguage"/> to an ISO-639-1 country code.</summary>
    public static string ToIso6391Code(this CaptchaLanguage language) => _dict[language];
}
