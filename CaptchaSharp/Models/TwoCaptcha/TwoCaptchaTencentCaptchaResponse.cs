using Newtonsoft.Json;

namespace CaptchaSharp.Models.TwoCaptcha;

internal class TwoCaptchaTencentCaptchaResponse : TwoCaptchaResponse
{
    public new TencentCaptchaSolution? Request { get; set; }
}

internal class TencentCaptchaSolution
{
    [JsonProperty("appid")]
    public required string AppId { get; init; }
    
    [JsonProperty("ticket")]
    public required string Ticket { get; init; }
    
    [JsonProperty("ret")]
    public required int ReturnCode { get; init; }
    
    [JsonProperty("randstr")]
    public required string RandomString { get; init; }

    public TencentCaptchaResponse ToTencentCaptchaResponse(string id)
    {
        return new TencentCaptchaResponse
        {
            Id = id,
            AppId = AppId,
            Ticket = Ticket,
            ReturnCode = ReturnCode,
            RandomString = RandomString
        };
    }
}
