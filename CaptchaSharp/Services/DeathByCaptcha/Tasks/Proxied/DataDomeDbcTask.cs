using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied;

internal class DataDomeDbcTask : DbcTask
{
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("captcha_url")]
    public required string CaptchaUrl { get; set; }
}
