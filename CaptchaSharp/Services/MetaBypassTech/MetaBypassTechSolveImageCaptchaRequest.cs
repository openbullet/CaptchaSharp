using Newtonsoft.Json;

namespace CaptchaSharp.Services.MetaBypassTech;

internal class MetaBypassTechSolveImageCaptchaRequest
{
    [JsonProperty("image")]
    public required string Base64Image { get; set; }
    
    [JsonProperty("numeric")]
    public int Numeric { get; set; }
    
    [JsonProperty("min_len")]
    public int MinLength { get; set; }
    
    [JsonProperty("max_len")]
    public int MaxLength { get; set; }
}
