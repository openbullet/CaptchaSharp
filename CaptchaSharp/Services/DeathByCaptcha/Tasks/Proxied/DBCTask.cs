using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using System;

namespace CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied
{
    internal class DBCTask : DBCTaskProxyless
    {
        public string Proxy { get; set; }
        public string ProxyType { get; set; } = "HTTP";

        public DBCTask SetProxy(Proxy proxy)
        {
            if (proxy.Type != Enums.ProxyType.HTTP && proxy.Type != Enums.ProxyType.HTTPS)
                throw new NotSupportedException($"DBC only supports HTTP proxies");

            if (proxy.RequiresAuthentication)
                Proxy = $"http://{proxy.Username}:{proxy.Password}@{proxy.Host}:{proxy.Port}";
            else
                Proxy = $"http://{proxy.Host}:{proxy.Port}";

            return this;
        }
    }
}
