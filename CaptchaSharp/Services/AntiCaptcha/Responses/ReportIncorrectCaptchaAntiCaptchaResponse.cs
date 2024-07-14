using Newtonsoft.Json;

namespace CaptchaSharp.Services.AntiCaptcha.Responses;

internal class ReportIncorrectCaptchaAntiCaptchaResponse
{
    public int ErrorId { get; set; }
    public required string Status { get; set; }

    [JsonIgnore]
    public bool Success => ErrorId == 0;

    [JsonIgnore]
    public bool NotFoundOrExpired => ErrorId == 16;
}
