using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks;

internal class RecaptchaV2DbcTaskProxyless : DbcTaskProxyless
{
    [JsonProperty("googlekey")]
    public required string GoogleKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
