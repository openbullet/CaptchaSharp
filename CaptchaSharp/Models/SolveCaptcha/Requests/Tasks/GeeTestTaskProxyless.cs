using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks;

internal class GeeTestTaskProxyless : SolveCaptchaTaskProxyless
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("gt")]
    public required string Gt { get; set; }
    
    [JsonProperty("challenge")]
    public required string Challenge { get; set; }
    
    [JsonProperty("geetestApiServerSubdomain", NullValueHandling = NullValueHandling.Ignore)]
    public string? GeeTestApiServerSubdomain { get; set; }
    
    public GeeTestTaskProxyless()
    {
        Method = "GeeTestTaskProxyless";
    }
}
