using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Models.MetaBypassTech;

internal class MbtResponse
{
    [JsonProperty("ok")]
    public required bool Ok { get; set; }
    
    [JsonProperty("data")]
    public JToken? Data { get; set; }
    
    [JsonProperty("status_code")]
    public int StatusCode { get; set; }
    
    [JsonProperty("message")]
    public string? Message { get; set; }
}
