using Newtonsoft.Json;

namespace CaptchaSharp.Models.Aycd.Requests.RenderParameters;

internal class AycdImageCaptchaRenderParameters : AycdRenderParameters
{
    [JsonProperty("base64ImageData")]
    public required string Base64ImageData { get; set; }
    
    /// <summary>
    /// This is a boolean... because this API is inconsistent as hell!
    /// </summary>
    [JsonProperty("caseSensitive")]
    public string CaseSensitive { get; set; } = "false";
}
