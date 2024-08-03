using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks;

internal class DataDomeDbcTask : DbcTask
{
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("captcha_url")]
    public required string CaptchaUrl { get; set; }
}
