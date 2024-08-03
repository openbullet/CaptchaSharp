using Newtonsoft.Json;

namespace CaptchaSharp.Models.MetaBypassTech;

internal class MbtSolveImageCaptchaRequest
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
