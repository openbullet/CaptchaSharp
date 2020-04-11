namespace CaptchaSharp.Models
{
    public class GeeTestResponse : CaptchaResponse
    {
        public string Challenge { get; set; }
        public string Validate { get; set; }
        public string SecCode { get; set; }
    }
}
