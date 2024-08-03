using System;
using System.Collections.Generic;
using System.Linq;

namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks.Proxied;

internal class AntiCaptchaTask : AntiCaptchaTaskProxyless
{
    public string? ProxyType { get; set; }
    public string? ProxyAddress { get; set; }
    public int ProxyPort { get; set; }
    public string? ProxyLogin { get; set; }
    public string? ProxyPassword { get; set; }
    public string? UserAgent { get; set; }
        
    // Format cookiename1=cookievalue1; cookiename2=cookievalue2
    public string? Cookies { get; set; }

    public AntiCaptchaTask WithSessionParams(SessionParams sessionParams)
    {
        UserAgent = sessionParams.UserAgent;
        SetCookies(sessionParams.Cookies);
        
        var proxy = sessionParams.Proxy;

        if (proxy is null)
        {
            return this;
        }
            
        if (!System.Net.IPAddress.TryParse(proxy.Host, out _))
        {
            throw new NotSupportedException(
                "Only IP addresses are supported for the proxy host");   
        }

        ProxyAddress = proxy.Host;
        ProxyPort = proxy.Port;
        ProxyType = proxy.Type.ToString().ToLower();
        ProxyLogin = proxy.Username;
        ProxyPassword = proxy.Password;

        return this;
    }

    private void SetCookies(Dictionary<string, string>? cookies)
    {
        if (cookies == null)
        {
            return;
        }

        Cookies = string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}"));
    }
}
