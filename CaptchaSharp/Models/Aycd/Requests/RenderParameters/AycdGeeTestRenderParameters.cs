using Newtonsoft.Json;

namespace CaptchaSharp.Models.Aycd.Requests.RenderParameters;

internal class AycdGeeTestRenderParameters : AycdRenderParameters
{
    [JsonProperty("challenge")]
    public required string Challenge { get; set; }
    
    [JsonProperty("api_server")]
    public required string ApiServer { get; set; }
}
