using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Responses;

/// <summary>
/// Represents the response from the SolveCaptcha API.
/// </summary>
public class SolveCaptchaResponse
{
    /// <summary>
    /// The ID of the error.
    /// </summary>
    [JsonProperty("error_id")]
    public int ErrorId { get; set; }
    
    /// <summary>
    /// The error code.
    /// </summary>
    [JsonProperty("error_code")]
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// The error description.
    /// </summary>
    [JsonProperty("error_description")]
    public string? ErrorDescription { get; set; }
    
    /// <summary>
    /// Whether the response is an error.
    /// </summary>
    [JsonIgnore]
    public bool IsError => ErrorId > 0;
}
