using Newtonsoft.Json;

namespace CaptchaSharp.Services.BestCaptchaSolver.Requests;

internal class BcsSolveImageRequest : BcsRequest
{
    [JsonProperty("b64image")]
    public required string Base64Image { get; set; }
    
    [JsonProperty("is_case", NullValueHandling = NullValueHandling.Ignore)]
    public bool? CaseSensitive { get; set; }
    
    [JsonProperty("is_phrase", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsPhrase { get; set; }
    
    [JsonProperty("is_math", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsMath { get; set; }
    
    [JsonProperty("alphanumeric", NullValueHandling = NullValueHandling.Ignore)]
    public int? Alphanumeric { get; set; }
    
    [JsonProperty("minlength", NullValueHandling = NullValueHandling.Ignore)]
    public int? MinLength { get; set; }
    
    [JsonProperty("maxlength", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxLength { get; set; }
}
