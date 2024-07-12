namespace CaptchaSharp.Models;

/// <summary>The solution of a GeeTest captcha.</summary>
public class GeeTestResponse : CaptchaResponse
{
    /// <summary></summary>
    public required string Challenge { get; init; }

    /// <summary></summary>
    public required string Validate { get; init; }

    /// <summary></summary>
    public required string SecCode { get; init; }
}
