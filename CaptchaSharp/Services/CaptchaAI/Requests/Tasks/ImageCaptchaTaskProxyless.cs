namespace CaptchaSharp.Services.CaptchaAI.Requests.Tasks
{
    internal class ImageCaptchaTask : CaptchaAITaskProxyless
    {
        public string Body { get; set; } = "";
        public bool Phrase { get; set; } = false;
        public bool Case { get; set; } = false;
        public int Numeric { get; set; } = 0;
        public bool Math { get; set; } = false;
        public int MinLength { get; set; } = 0;
        public int MaxLength { get; set; } = 0;
        public string Comment { get; set; } = "";

        public ImageCaptchaTask()
        {
            Type = "ImageToTextTask";
        }
    }
}
