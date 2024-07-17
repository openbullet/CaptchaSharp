using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied;

internal class RecaptchaV2DbcTask : DbcTask
{
    [JsonProperty("googlekey")]
    public required string GoogleKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
