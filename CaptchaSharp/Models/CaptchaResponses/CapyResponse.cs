namespace CaptchaSharp.Models.CaptchaResponses;

/// <summary>The solution of a Capy captcha.</summary>
public class CapyResponse : CaptchaResponse
{
    /// <summary></summary>
    public required string CaptchaKey { get; init; }

    /// <summary></summary>
    public required string ChallengeKey { get; init; }

    /// <summary></summary>
    public required string Answer { get; init; }
}
