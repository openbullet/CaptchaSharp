using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks.Proxied;

internal class GeeTestTask : SolveCaptchaTask
{
    [JsonProperty("page_url")]
    public required string PageUrl { get; set; }
    
    [JsonProperty("gt")]
    public required string Gt { get; set; }
    
    [JsonProperty("challenge")]
    public required string Challenge { get; set; }
    
    [JsonProperty("geetestApiServerSubdomain", NullValueHandling = NullValueHandling.Ignore)]
    public string? GeeTestApiServerSubdomain { get; set; }
    
    public GeeTestTask()
    {
        Method = "GeeTestTask";
    }
}
