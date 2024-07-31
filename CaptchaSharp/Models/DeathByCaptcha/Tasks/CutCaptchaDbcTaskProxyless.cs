using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks;

internal class CutCaptchaDbcTaskProxyless : DbcTaskProxyless
{
    [JsonProperty("apikey")]
    public required string ApiKey { get; set; }
    
    [JsonProperty("miserykey")]
    public required string MiseryKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
