using Newtonsoft.Json;

namespace CaptchaSharp.Models.AntiCaptcha.Responses;

public class AntiCaptchaResponse
{
    public int ErrorId { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorDescription { get; set; }

    [JsonIgnore]
    public bool IsError => ErrorId > 0;
}
