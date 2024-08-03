using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;

internal class RecaptchaV2EnterpriseTask : CapSolverTask
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
        
    [JsonProperty("enterprisePayload", NullValueHandling = NullValueHandling.Ignore)]
    public JObject? EnterprisePayload { get; set; }

    public RecaptchaV2EnterpriseTask()
    {
        Type = "RecaptchaV2EnterpriseTask";
    }
}
