using CaptchaSharp.Enums;

namespace CaptchaSharp.Models;

/// <summary>Provides additional options for an image based captcha task.</summary>
public class ImageCaptchaOptions
{
    /// <summary>Whether the captcha is made of multiple words.</summary>
    public bool IsPhrase { get; init; }

    /// <summary>Whether the captcha should be solved as case-sensitive.</summary>
    public bool CaseSensitive { get; init; } = true;

    /// <summary>The set of allowed characters.</summary>
    public CharacterSet CharacterSet { get; init; } = CharacterSet.NotSpecified;

    /// <summary>Whether the captcha includes mathematical calculations.</summary>
    public bool RequiresCalculation { get; init; }

    /// <summary>The minimum possible length of the text.</summary>
    public int MinLength { get; init; }

    /// <summary>The maximum possible length of the text.</summary>
    public int MaxLength { get; init; }

    /// <summary>The language group of the text.</summary>
    public CaptchaLanguageGroup CaptchaLanguageGroup { get; init; } = CaptchaLanguageGroup.NotSpecified;

    /// <summary>The language of the text.</summary>
    public CaptchaLanguage CaptchaLanguage { get; init; } = CaptchaLanguage.NotSpecified;

    /// <summary>Any additional text instruction (e.g. type the characters in red).</summary>
    public string TextInstructions { get; init; } = string.Empty;
}
