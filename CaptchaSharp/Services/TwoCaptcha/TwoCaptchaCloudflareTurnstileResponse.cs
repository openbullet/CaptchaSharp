using CaptchaSharp.Models;
using Newtonsoft.Json;

namespace CaptchaSharp.Services.TwoCaptcha;

internal class TwoCaptchaCloudflareTurnstileResponse : TwoCaptchaResponse
{
    /// <summary>
    /// The User-Agent used to solve the challenge.
    /// </summary>
    [JsonProperty("useragent")]
    public string? UserAgent { get; set; }

    public CloudflareTurnstileResponse ToCloudflareTurnstileResponse(long id)
    {
        return new CloudflareTurnstileResponse()
        {
            Id = id,
            Response = Request,
            UserAgent = UserAgent
        };
    }
}
