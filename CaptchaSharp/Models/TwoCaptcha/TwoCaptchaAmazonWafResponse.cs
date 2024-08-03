using CaptchaSharp.Models.CaptchaResponses;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.TwoCaptcha;

internal class TwoCaptchaAmazonWafResponse : TwoCaptchaResponse
{
    public new AmazonWafSolution? Request { get; set; }
}

internal class AmazonWafSolution
{
    [JsonProperty("captcha_voucher")]
    public string? CaptchaVoucher { get; set; }

    [JsonProperty("existing_token")]
    public required string ExistingToken { get; set; }

    public StringResponse ToStringResponse(string id)
    {
        return new StringResponse
        {
            Id = id,
            Response = ExistingToken
        };
    }
}
