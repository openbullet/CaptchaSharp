using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks.Proxied;

internal class LeminCroppedDbcTask : DbcTask
{
    [JsonProperty("captchaid")]
    public required string CaptchaId { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
