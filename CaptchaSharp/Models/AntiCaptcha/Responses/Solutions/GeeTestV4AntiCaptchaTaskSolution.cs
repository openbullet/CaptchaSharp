using Newtonsoft.Json;

namespace CaptchaSharp.Models.AntiCaptcha.Responses.Solutions;

internal class GeeTestV4AntiCaptchaTaskSolution : AntiCaptchaTaskSolution
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
    
    public override CaptchaResponse ToCaptchaResponse(string id)
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
