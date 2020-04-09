using Newtonsoft.Json;

namespace CaptchaSharp.Services.TwoCaptcha
{
    public class TwoCaptchaResponse
    {
        public int Status { get; set; }
        public string Request { get; set; }
        public string Error_Text { get; set; }

        [JsonIgnore]
        public bool Success => Status == 1;

        [JsonIgnore]
        public bool IsErrorCode => Status == 0 && Request.Contains("ERROR");
    }
}
