using CaptchaSharp.Models.CaptchaResponses;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.TwoCaptcha;

internal class TwoCaptchaCloudflareTurnstileResponse : TwoCaptchaResponse
{
    /// <summary>
    /// The User-Agent used to solve the challenge.
    /// </summary>
    [JsonProperty("useragent")]
    public string? UserAgent { get; set; }

    public CloudflareTurnstileResponse ToCloudflareTurnstileResponse(string id)
    {
        return new CloudflareTurnstileResponse()
        {
            Id = id,
            Response = Request!,
            UserAgent = UserAgent
        };
    }
}
