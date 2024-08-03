using Newtonsoft.Json;

namespace CaptchaSharp.Models.Nopecha;

internal class NopechaSolveImageRequest : NopechaRequest
{
    [JsonProperty("type")]
    public string Type => "textcaptcha";
    
    [JsonProperty("image_data")]
    public string[] ImageData { get; set; } = [];
}
