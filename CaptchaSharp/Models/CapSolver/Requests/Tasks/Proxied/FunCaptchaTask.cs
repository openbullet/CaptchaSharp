using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;

internal class FunCaptchaTask : CapSolverTask
{
    [JsonProperty("websiteURL")]
    public required string WebsiteUrl { get; set; }
    
    [JsonProperty("websitePublicKey")]
    public required string WebsitePublicKey { get; set; }
    
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public string? Data { get; set; }

    public FunCaptchaTask()
    {
        Type = "FunCaptchaTask";
    }
}
