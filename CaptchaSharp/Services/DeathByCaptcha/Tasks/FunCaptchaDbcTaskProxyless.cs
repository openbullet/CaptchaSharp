﻿using Newtonsoft.Json;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks;

internal class FunCaptchaDbcTaskProxyless : DbcTaskProxyless
{
    [JsonProperty("publickey")]
    public required string PublicKey { get; set; }
        
    [JsonProperty("pageurl")]
    public required string PageUrl { get; set; }
}