using Newtonsoft.Json;

namespace CaptchaSharp.Models.Aycd.Responses;

internal class AycdTask
{
    [JsonProperty("taskId")]
    public required string TaskId { get; set; }
    
    [JsonProperty("createdAt")]
    public required long CreatedAt { get; set; }
    
    [JsonProperty("status")]
    public required string Status { get; set; }
    
    [JsonProperty("token")]
    public string? Token { get; set; }
}
