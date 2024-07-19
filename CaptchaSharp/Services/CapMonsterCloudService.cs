using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.AntiCaptcha.Responses;
using CaptchaSharp.Services.CapMonsterCloud.Requests.Tasks;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by <c>https://capmonster.cloud/</c>
/// </summary>
public class CapMonsterCloudService : CustomAntiCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="CapMonsterCloudService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public CapMonsterCloudService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("https://api.capmonster.cloud"), httpClient)
    {
        SupportedCaptchaTypes = 
            Enums.CaptchaType.ImageCaptcha |
            Enums.CaptchaType.ReCaptchaV2 |
            Enums.CaptchaType.ReCaptchaV3 |
            Enums.CaptchaType.HCaptcha |
            Enums.CaptchaType.GeeTest |
            Enums.CaptchaType.CloudflareTurnstile |
            Enums.CaptchaType.DataDome;
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveDataDomeAsync(
        string siteUrl, string captchaUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        // CapMonsterCloud will always use the current Windows OS User-Agent
        // to solve captchas.
        
        if (proxy?.Cookies is null)
        {
            throw new ArgumentNullException(
                nameof(proxy), "DataDome requires cookies");
        }
        
        // The cookie must contain datadome=... and nothing else
        var datadomeCookie = Array.Find(proxy.Cookies, c => c.Item1 == "datadome").Item2;
        
        if (string.IsNullOrEmpty(datadomeCookie) || proxy.Cookies.Length > 1)
        {
            throw new ArgumentException(
                "The cookie must contain a single datadome cookie", nameof(proxy));
        }

        var content = CreateTaskRequest();

        content.Task = new DataDomeTaskProxyless
        {
            WebsiteURL = siteUrl,
            Metadata = new DataDomeMetadata
            {
                CaptchaUrl = captchaUrl,
                DataDomeCookie = $"datadome={datadomeCookie}"
            }
        };
        
        var response = await HttpClient.PostJsonToStringAsync(
                "createTask", 
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response.Deserialize<TaskCreationAntiCaptchaResponse>(), CaptchaType.DataDome,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override StringResponse ParseDataDomeSolution(JToken? solution)
    {
        // The solution is like { "domains": { "site.com": { "cookies": { "datadome": "..." } } } }
        // We want to return the datadome cookie. Since site.com varies, we need to
        // take the first domain.
        var cookie = solution
            ?.SelectToken("domains")
            ?.First?.First
            ?.SelectToken("cookies.datadome")
            ?.Value<string>() ?? "";
        
        return new StringResponse { Response = cookie };
    }
}
