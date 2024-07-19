using Newtonsoft.Json;

namespace CaptchaSharp.Services.Nopecha;

internal class NopechaSolveImageRequest : NopechaRequest
{
    [JsonProperty("type")]
    public string Type => "textcaptcha";
    
    [JsonProperty("image_data")]
    public string[] ImageData { get; set; } = [];
}
