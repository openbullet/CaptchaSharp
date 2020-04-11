using Newtonsoft.Json;

namespace CaptchaSharp.Services.AntiCaptcha.Responses
{
    internal class Response
    {
        public int ErrorId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }

        [JsonIgnore]
        public bool IsError => ErrorId > 0;
    }
}
