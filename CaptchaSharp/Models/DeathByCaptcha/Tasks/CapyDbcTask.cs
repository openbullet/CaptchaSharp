using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks;

internal class CapyDbcTask : DbcTask
{
    [JsonProperty("captchakey")]
    public required string CaptchaKey { get; set; }
    
    [JsonProperty("api_server")]
    public string ApiServer { get; set; } = "https://www.capy.me/";
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
