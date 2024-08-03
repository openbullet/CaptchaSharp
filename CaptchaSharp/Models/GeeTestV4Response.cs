namespace CaptchaSharp.Models;

/// <summary>The solution of a GeeTest v4 captcha.</summary>
public class GeeTestV4Response : CaptchaResponse
{
    /// <summary></summary>
    public required string CaptchaId { get; init; }
    
    /// <summary></summary>
    public required string LotNumber { get; init; }
    
    /// <summary></summary>
    public required string PassToken { get; init; }
    
    /// <summary></summary>
    public required string GenTime { get; init; }
    
    /// <summary></summary>
    public required string CaptchaOutput { get; init; }
}
