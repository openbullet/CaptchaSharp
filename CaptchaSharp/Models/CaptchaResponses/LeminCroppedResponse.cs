namespace CaptchaSharp.Models.CaptchaResponses;

/// <summary>
/// A captcha response for Lemin Cropped Captchas.
/// </summary>
public class LeminCroppedResponse : CaptchaResponse
{
    /// <summary>
    /// The answer to the challenge.
    /// </summary>
    public required string Answer { get; set; }
    
    /// <summary>
    /// The challenge ID.
    /// </summary>
    public required string ChallengeId { get; set; }
}
