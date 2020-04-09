namespace CaptchaSharp.Models
{
    public struct CaptchaResponse
    {
        public string Id;
        public string Response;

        public CaptchaResponse(string id, string response)
        {
            Id = id;
            Response = response;
        }
    }
}
