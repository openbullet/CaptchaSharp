using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapSolver.Requests;

internal class CaptchaTaskFeedbackRequest : Request
{
    public required string AppId { get; set; }
    
    public required string TaskId { get; set; }
    
    public required TaskResultFeedback Result { get; set; }
}

internal class TaskResultFeedback
{
    public required bool Invalid { get; set; }
    
    [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
    public int? Code { get; set; }
    
    [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
    public string? Message { get; set; }
}
