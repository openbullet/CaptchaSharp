using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Responses;

internal class TaskCreationSolveCaptchaResponse : SolveCaptchaResponse
{
    [JsonProperty("task_id")]
    public long TaskId { get; set; }
}
