using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks;

internal class KeyCaptchaDbcTaskProxyless : DbcTaskProxyless
{
    [JsonProperty("s_s_c_user_id")]
    public required string UserId { get; set; }
    
    [JsonProperty("s_s_c_session_id")]
    public required string SessionId { get; set; }
    
    [JsonProperty("s_s_c_web_server_sign")]
    
    public required string WebServerSign { get; set; }
    
    [JsonProperty("s_s_c_web_server_sign2")]
    public required string WebServerSign2 { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}
