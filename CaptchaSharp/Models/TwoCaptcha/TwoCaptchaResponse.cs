using Newtonsoft.Json;

namespace CaptchaSharp.Models.TwoCaptcha;

internal class TwoCaptchaResponse
{
    public int Status { get; set; }
    public string? Request { get; set; }
    
    [JsonProperty("Error_Text")]
    public string? ErrorText { get; set; }

    [JsonIgnore]
    public bool Success => Status == 1;

    [JsonIgnore]
    public bool IsErrorCode => 
        Status == 0 && Request is not null && Request.Contains("ERROR");
}
