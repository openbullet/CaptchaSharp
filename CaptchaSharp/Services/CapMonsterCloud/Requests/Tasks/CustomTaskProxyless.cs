using CaptchaSharp.Services.AntiCaptcha.Requests.Tasks;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.CapMonsterCloud.Requests.Tasks;

internal abstract class CustomTaskProxyless : AntiCaptchaTaskProxyless
{
    [JsonProperty("class")]
    public string Class { get; set; }

    protected CustomTaskProxyless()
    {
        Type = "CustomTask";
    }
}
