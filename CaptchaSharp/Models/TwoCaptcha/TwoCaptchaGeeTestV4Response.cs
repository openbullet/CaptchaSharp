using Newtonsoft.Json;

namespace CaptchaSharp.Models.TwoCaptcha;

internal class TwoCaptchaGeeTestV4Response : TwoCaptchaResponse
{
    public new GeeTestV4Solution? Request { get; set; }
}

internal class GeeTestV4Solution
{
    [JsonProperty("captcha_id")]
    public required string CaptchaId { get; set; }
    
    [JsonProperty("lot_number")]
    public required string LotNumber { get; set; }
    
    [JsonProperty("pass_token")]
    public required string PassToken { get; set; }
    
    [JsonProperty("gen_time")]
    public required string GenTime { get; set; }
    
    [JsonProperty("captcha_output")]
    public required string CaptchaOutput { get; set; }
    
    public GeeTestV4Response ToGeeTestV4Response(string id)
    {
        return new GeeTestV4Response
        {
            Id = id,
            CaptchaId = CaptchaId,
            LotNumber = LotNumber,
            PassToken = PassToken,
            GenTime = GenTime,
            CaptchaOutput = CaptchaOutput
        };
    }
}
