using Newtonsoft.Json;

namespace CaptchaSharp.Models.AntiCaptcha.Responses;

/// <summary>
/// Represents the response from the AntiCaptcha API.
/// </summary>
public class AntiCaptchaResponse
{
    /// <summary>
    /// The ID of the error.
    /// </summary>
    public int ErrorId { get; set; }
    
    /// <summary>
    /// The error code.
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// The error description.
    /// </summary>
    public string? ErrorDescription { get; set; }

    /// <summary>
    /// Whether the response is an error.
    /// </summary>
    [JsonIgnore]
    public bool IsError => ErrorId > 0;
}
