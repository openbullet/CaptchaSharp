using Newtonsoft.Json;

namespace CaptchaSharp.Models.EndCaptcha;

internal class FuncaptchaTokenParams : EndCaptchaTokenParams
{
    [JsonProperty("publickey")]
    public required string PublicKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
