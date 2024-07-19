using CaptchaSharp.Services.AntiCaptcha.Requests.Tasks;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.AntiCaptcha.Requests;

/// <summary>
/// A request to solve a captcha task.
/// </summary>
public class CaptchaTaskRequest : Request
{
    /// <summary>
    /// The task to solve.
    /// </summary>
    public AntiCaptchaTaskProxyless Task { get; set; } = null!;
    
    /// <summary>
    /// The soft ID to use. Default is 0.
    /// </summary>
    [JsonProperty("softId", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int? SoftId { get; set; }

    /// <summary>
    /// The language pool to use. Default is "en".
    /// </summary>
    [JsonProperty("languagePool", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? LanguagePool { get; set; }
}
