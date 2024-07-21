using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks.Proxied;

internal class CapyDbcTask : DbcTask
{
    [JsonProperty("captchakey")]
    public required string CaptchaKey { get; set; }
    
    [JsonProperty("api_server")]
    public string ApiServer { get; set; } = "https://www.capy.me/";
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
