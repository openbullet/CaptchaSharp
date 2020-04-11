using Newtonsoft.Json;

namespace CaptchaSharp.Services.AntiCaptcha.Responses
{
    internal class ReportIncorrectCaptchaResponse
    {
        public int ErrorId { get; set; }
        public string Status { get; set; }

        [JsonIgnore]
        public bool Success => ErrorId == 0;

        [JsonIgnore]
        public bool NotFoundOrExpired => ErrorId == 16;
    }
}
