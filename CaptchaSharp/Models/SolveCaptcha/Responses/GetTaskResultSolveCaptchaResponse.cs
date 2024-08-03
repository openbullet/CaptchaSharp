using CaptchaSharp.Models.SolveCaptcha.Responses.Solutions;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Responses;

internal class GetTaskResultSolveCaptchaResponse : SolveCaptchaResponse
{
    [JsonProperty("status")]
    public string? Status { get; set; }
    
    [JsonProperty("solution")]
    public SolveCaptchaTaskSolution? SolveCaptchaTaskSolution { get; set; }
    
    [JsonIgnore]
    public bool IsReady => Status != "processing";
}
