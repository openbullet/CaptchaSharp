using System.Linq;
using Newtonsoft.Json;

namespace CaptchaSharp.Models.SolveCaptcha.Requests.Tasks;

/// <summary>
/// A task that does not require a proxy.
/// </summary>
public class SolveCaptchaTaskProxyless
{
    /// <summary>
    /// The method to use.
    /// </summary>
    [JsonProperty("method")]
    public string? Method { get; set; }
    
    /// <summary>
    /// The user agent to use.
    /// </summary>
    [JsonProperty("user_agent", NullValueHandling = NullValueHandling.Ignore)]
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// The cookies to use, formatted like "name1=value1; name2=value2".
    /// </summary>
    [JsonProperty("cookies", NullValueHandling = NullValueHandling.Ignore)]
    public string? Cookies { get; set; }

    /// <summary>
    /// Set some parameters from a proxy.
    /// </summary>
    public virtual void SetParamsFromProxy(Proxy? proxy)
    {
        if (proxy is null)
        {
            return;
        }
        
        UserAgent = proxy.UserAgent;
        
        if (proxy.Cookies == null)
        {
            return;
        }

        Cookies = string.Join("; ", proxy.Cookies.Select(c => $"{c.Name}={c.Value}"));
    }
}
