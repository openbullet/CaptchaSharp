using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapSolver.Requests.Tasks;

internal class AntiAwsWafTaskProxyless : CapSolverTaskProxyless
{
    [JsonProperty("websiteURL")]
    public required string WebsiteURL { get; set; }
    
    [JsonProperty("awsKey")]
    public required string AwsKey { get; set; }
    
    [JsonProperty("awsIv")]
    public required string AwsIv { get; set; }
    
    [JsonProperty("awsContext")]
    public required string AwsContext { get; set; }
    
    [JsonProperty("awsChallengeJS", NullValueHandling = NullValueHandling.Ignore)]
    public string? AwsChallengeJs { get; set; }

    public AntiAwsWafTaskProxyless()
    {
        Type = "AntiAwsWafTaskProxyLess";
    }
}
