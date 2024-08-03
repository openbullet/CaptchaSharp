using CaptchaSharp.Models.EzCaptcha.Requests.Tasks;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.EzCaptcha.Requests;

/// <summary>
/// A request to solve a captcha task.
/// </summary>
public class CaptchaTaskEzCaptchaRequest : EzCaptchaRequest
{
    /// <summary>
    /// The task to solve.
    /// </summary>
    public EzCaptchaTaskProxyless Task { get; set; } = null!;
    
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

