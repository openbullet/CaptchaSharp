using CaptchaSharp.Models.CapSolver.Requests.Tasks;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapSolver.Requests;

internal class CaptchaTaskRequest : Request
{
    public CapSolverTaskProxyless? Task { get; set; }
    public required string AppId { get; set; }
    
    [JsonProperty("languagePool", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string? LanguagePool { get; set; }
}
