using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied;

internal class FunCaptchaDbcTask : DbcTask
{
    [JsonProperty("publickey")]
    public required string PublicKey { get; set; }
        
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
