using CaptchaSharp.Models.SolveCaptcha.Requests.Tasks;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests;

internal class CaptchaTaskSolveCaptchaRequest : SolveCaptchaRequest
{
    [JsonProperty("task")]
    public SolveCaptchaTaskProxyless? Task { get; set; }
}
