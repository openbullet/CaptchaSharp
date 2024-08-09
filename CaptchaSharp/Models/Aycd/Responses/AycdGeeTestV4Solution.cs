using CaptchaSharp.Models.CaptchaResponses;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.Aycd.Responses;

internal class AycdGeeTestV4Solution
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
    
    public GeeTestV4Response ToGeeTestV4Response(string taskId)
    {
        return new GeeTestV4Response
        {
            Id = taskId,
            CaptchaId = CaptchaId,
            LotNumber = LotNumber,
            PassToken = PassToken,
            GenTime = GenTime,
            CaptchaOutput = CaptchaOutput
        };
    }
}
