using CaptchaSharp.Models.CaptchaResponses;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.Aycd.Responses;

internal class AycdGeeTestSolution
{
    [JsonProperty("challenge")]
    public required string Challenge { get; set; }
    
    [JsonProperty("validate")]
    public required string Validate { get; set; }
    
    [JsonProperty("seccode")]
    public required string SecCode { get; set; }
    
    public GeeTestResponse ToGeeTestResponse(string taskId)
    {
        return new GeeTestResponse
        {
            Id = taskId,
            Challenge = Challenge,
            Validate = Validate,
            SecCode = SecCode
        };
    }
}
