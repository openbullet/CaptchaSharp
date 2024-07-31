using CaptchaSharp.Enums;

namespace CaptchaSharp.Models;

/// <summary>
/// Provides additional options for an audio based captcha task.
/// </summary>
public class AudioCaptchaOptions
{
    /// <summary>
    /// The language of the audio.
    /// </summary>
    public CaptchaLanguage CaptchaLanguage { get; init; } = CaptchaLanguage.English;
}
