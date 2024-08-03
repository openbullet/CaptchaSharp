namespace CaptchaSharp.Models.CaptchaResponses;

/// <summary>A captcha response with a string solution.</summary>
public class StringResponse : CaptchaResponse
{
    /// <summary>The plaintext response string.</summary>
    public required string Response { get; init; }
}
