using Newtonsoft.Json;

namespace CaptchaSharp.Services.CapSolver.Requests.Tasks;

internal class AntiTurnstileTaskProxyless : CapSolverTaskProxyless
{
    public required string WebsiteKey { get; set; }
    public required string WebsiteURL { get; set; }
    public TurnstileMetadata? Metadata { get; set; }

    public AntiTurnstileTaskProxyless()
    {
        Type = "AntiTurnstileTaskProxyLess";
    }
}

internal class TurnstileMetadata
{
    [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
    public string? Action { get; set; }
    
    [JsonProperty("cdata", NullValueHandling = NullValueHandling.Ignore)]
    public string? CData { get; set; }
}
