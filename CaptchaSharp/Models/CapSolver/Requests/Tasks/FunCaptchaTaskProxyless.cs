using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapSolver.Requests.Tasks;

internal class FunCaptchaTaskProxyless : CapSolverTaskProxyless
{
    [JsonProperty("websiteURL")]
    public required string WebsiteUrl { get; set; }
    
    [JsonProperty("websitePublicKey")]
    public required string WebsitePublicKey { get; set; }
    
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public string? Data { get; set; }

    public FunCaptchaTaskProxyless()
    {
        Type = "FunCaptchaTaskProxyless";
    }
}
