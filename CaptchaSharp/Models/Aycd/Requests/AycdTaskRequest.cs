using System;
using System.Linq;
using CaptchaSharp.Models.Aycd.Requests.RenderParameters;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.Aycd.Requests;

internal class AycdTaskRequest
{
    [JsonProperty("taskId")]
    public string TaskId { get; set; } = Guid.NewGuid().ToString();
    
    [JsonProperty("version")]
    public required int Version { get; set; }
    
    /// <summary>
    /// Always required, even when it doesn't make sense.
    /// </summary>
    [JsonProperty("url")]
    public required string Url { get; set; }
    
    /// <summary>
    /// Always required, even when it doesn't make sense.
    /// </summary>
    [JsonProperty("siteKey")]
    public required string SiteKey { get; set; }
    
    /// <summary>
    /// Only for reCAPTCHA v3.
    /// </summary>
    [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
    public string? Action { get; set; }
    
    /// <summary>
    /// Only for reCAPTCHA v3.
    /// </summary>
    [JsonProperty("minScore", NullValueHandling = NullValueHandling.Ignore)]
    public double? MinScore { get; set; }
    
    [JsonProperty("renderParameters", NullValueHandling = NullValueHandling.Ignore)]
    public AycdRenderParameters? RenderParameters { get; set; }
    
    /// <summary>
    /// Format = IP:PORT:USERNAME:PASSWORD
    /// </summary>
    [JsonProperty("proxy", NullValueHandling = NullValueHandling.Ignore)]
    public string? Proxy { get; set; }
    
    [JsonProperty("proxyRequired", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ProxyRequired { get; set; }
    
    [JsonProperty("userAgent", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// Format = key1=value1;key2=value2
    /// </summary>
    [JsonProperty("cookies", NullValueHandling = NullValueHandling.Ignore)]
    public string? Cookies { get; set; }
    
    public AycdTaskRequest WithSessionParams(SessionParams? sessionParams)
    {
        if (sessionParams is null)
        {
            return this;
        }
        
        UserAgent = sessionParams.UserAgent;

        if (sessionParams.Cookies is not null && sessionParams.Cookies.Any())
        {
            Cookies = string.Join(";", sessionParams.Cookies.Select(c => $"{c.Key}={c.Value}"));
        }
        
        var proxy = sessionParams.Proxy;
        
        if (proxy is null)
        {
            return this;
        }
        
        Proxy = proxy.RequiresAuthentication
            ? $"{proxy.Host}:{proxy.Port}:{proxy.Username}:{proxy.Password}"
            : $"{proxy.Host}:{proxy.Port}";

        ProxyRequired = true;

        return this;
    }
}
