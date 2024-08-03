namespace CaptchaSharp.Models.CaptchaResponses;

/// <summary>The solution of a Tencent captcha.</summary>
public class TencentCaptchaResponse : CaptchaResponse
{
    /// <summary>The ID of the app that requested the captcha.</summary>
    public required string AppId { get; init; }
    
    /// <summary>The ticket that needs to be sent to the server to verify the captcha.</summary>
    public required string Ticket { get; init; }
    
    /// <summary>The return code of the captcha.</summary>
    public required int ReturnCode { get; init; }
    
    /// <summary>A random string that needs to be sent to the server to verify the captcha.</summary>
    public required string RandomString { get; init; }
}
