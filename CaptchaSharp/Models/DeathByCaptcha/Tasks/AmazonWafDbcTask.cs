using Newtonsoft.Json;

namespace CaptchaSharp.Models.DeathByCaptcha.Tasks;

internal class AmazonWafDbcTask : DbcTask
{
    [JsonProperty("sitekey")]
    public required string SiteKey { get; set; }
    
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("iv")]
    public required string Iv { get; set; }
    
    [JsonProperty("context")]
    public required string Context { get; set; }
    
    [JsonProperty("challengejs")]
    public string? ChallengeJs { get; set; }
    
    [JsonProperty("captchajs")]
    public string? CaptchaJs { get; set; }
}
