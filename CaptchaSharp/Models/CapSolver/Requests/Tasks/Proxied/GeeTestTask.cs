﻿using Newtonsoft.Json;

namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;

internal class GeeTestTask : CapSolverTask
{
    [JsonProperty("websiteURL")]
    public string? WebsiteURL { get; set; }
    
    [JsonProperty("gt", NullValueHandling = NullValueHandling.Ignore)]
    public string? Gt { get; set; }
    
    [JsonProperty("challenge", NullValueHandling = NullValueHandling.Ignore)]
    public string? Challenge { get; set; }
    
    [JsonProperty("geetestApiServerSubdomain", NullValueHandling = NullValueHandling.Ignore)]
    public string? GeetestApiServerSubdomain { get; set; }
    
    [JsonProperty("captchaId", NullValueHandling = NullValueHandling.Ignore)]
    public string? CaptchaId { get; set; }

    public GeeTestTask()
    {
        Type = "GeeTestTask";
    }
}
