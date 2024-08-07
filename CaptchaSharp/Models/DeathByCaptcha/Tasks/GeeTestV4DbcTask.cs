using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks;

internal class GeeTestV4DbcTask : DbcTask
{
    [JsonProperty("captcha_id")]
    public required string CaptchaId { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
