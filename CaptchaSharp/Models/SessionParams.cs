using System.Collections.Generic;
using System.Linq;

namespace CaptchaSharp.Models;

/// <summary>
/// Parameters of your session that can be sent to a captcha service to
/// use the same proxy, user agent and cookies as you do, and reduce the
/// detectability of your bot.
/// </summary>
public class SessionParams
{
    /// <summary>
    /// The proxy to use, if any. This proxy will be used by the
    /// captcha service to make requests to the target website, so
    /// the IP address will be the same as the one you use.
    /// </summary>
    public Proxy? Proxy { get; set; }
    
    /// <summary>
    /// The User-Agent to use, if any. This User-Agent will be used by
    /// the captcha service to make requests to the target website, so
    /// the User-Agent will be the same as the one you use.
    /// </summary>
    public string? UserAgent { get; set; }
    
    /// <summary>
    /// The cookies to use, if any. These cookies will be used by the
    /// captcha service to make requests to the target website, for example
    /// when you need to be logged in to access the page where the captcha
    /// is shown.
    /// </summary>
    public Dictionary<string, string>? Cookies { get; set; }
    
    internal string GetCookieString()
        => Cookies != null ? string.Join("; ", Cookies.Select(c => $"{c.Key}={c.Value}")) : string.Empty;
}
