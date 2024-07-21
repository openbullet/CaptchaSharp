using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Models.CapSolver.Requests.Tasks;

internal class RecaptchaV2EnterpriseTaskProxyless : CapSolverTaskProxyless
{
    public string WebsiteURL { get; set; }
    public string WebsiteKey { get; set; }
        
    [JsonProperty("enterprisePayload", NullValueHandling = NullValueHandling.Ignore)]
    public JObject? EnterprisePayload { get; set; }

    public RecaptchaV2EnterpriseTaskProxyless()
    {
        Type = "RecaptchaV2EnterpriseTaskProxyless";
    }
}
