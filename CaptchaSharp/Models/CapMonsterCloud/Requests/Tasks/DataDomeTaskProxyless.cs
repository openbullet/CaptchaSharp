using CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapMonsterCloud.Requests.Tasks;

internal class DataDomeTaskProxyless : CustomTaskProxyless
{   
    [JsonProperty("websiteURL")]
    public required string WebsiteURL { get; set; }
    
    [JsonProperty("metadata")]
    public required DataDomeMetadata Metadata { get; set; }
    
    [JsonProperty("userAgent")]
    public string? UserAgent { get; set; }

    public DataDomeTaskProxyless()
    {
        Class = "DataDome";
    }
}

internal class DataDomeMetadata
{
    [JsonProperty("htmlPageBase64", NullValueHandling=NullValueHandling.Ignore)]
    public string? HtmlPageBase64 { get; set; }
    
    [JsonProperty("captchaUrl", NullValueHandling=NullValueHandling.Ignore)]
    public string? CaptchaUrl { get; set; }
    
    [JsonProperty("datadomeCookie")]
    public required string DataDomeCookie { get; set; }
}
