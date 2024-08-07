using Newtonsoft.Json;

namespace CaptchaSharp.Models.BestCaptchaSolver.Responses;

internal class BcsBalanceResponse : BcsResponse
{
    [JsonProperty("balance")]
    public string? Balance { get; set; }  // This should be decimal, but the API returns a string
}
