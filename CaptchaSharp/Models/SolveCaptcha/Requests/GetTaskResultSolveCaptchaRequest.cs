using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests;

internal class GetTaskResultSolveCaptchaRequest : SolveCaptchaRequest
{
    [JsonProperty("task_id")]
    public long TaskId { get; set; }
}
