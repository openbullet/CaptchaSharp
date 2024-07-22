using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests;

/// <summary>
/// Base request for the SolveCaptcha service.
/// </summary>
public class SolveCaptchaRequest
{
    /// <summary>
    /// The soft ID to use. Default is 0.
    /// </summary>
    [JsonProperty("affiliate_id", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? AffiliateId { get; set; }
}
