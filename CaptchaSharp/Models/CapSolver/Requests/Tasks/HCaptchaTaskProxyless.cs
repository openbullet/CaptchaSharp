using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Models.CapSolver.Requests.Tasks;

internal class HCaptchaTaskProxyless : CapSolverTaskProxyless
{
    [JsonProperty("websiteKey")]
    public required string WebsiteKey { get; set; }
    
    [JsonProperty("websiteURL")]
    public required string WebsiteUrl { get; set; }
    
    [JsonProperty("isInvisible")]
    public bool IsInvisible { get; set; }
    
    [JsonProperty("enterprisePayload", NullValueHandling = NullValueHandling.Ignore)]
    public JObject? EnterprisePayload { get; set; }

    public HCaptchaTaskProxyless()
    {
        Type = "HCaptchaTaskProxyless";
    }
}
