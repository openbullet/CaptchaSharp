using Newtonsoft.Json;

namespace CaptchaSharp.Models.EndCaptcha;

internal class RecaptchaV2TokenParams : EndCaptchaTokenParams
{
    [JsonProperty("googlekey")]
    public required string GoogleKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
