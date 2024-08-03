using CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapMonsterCloud.Requests.Tasks;

internal abstract class CustomTaskProxyless : AntiCaptchaTaskProxyless
{
    [JsonProperty("class")]
    public string? Class { get; set; }

    protected CustomTaskProxyless()
    {
        Type = "CustomTask";
    }
}
