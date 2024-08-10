using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.AntiCaptcha.Responses;
using CaptchaSharp.Models.CapMonsterCloud.Requests.Tasks;
using CaptchaSharp.Models.CapMonsterCloud.Requests.Tasks.Proxied;
using CaptchaSharp.Models.CaptchaResponses;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://capmonster.cloud/
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
            CaptchaType.ImageCaptcha |
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3 |
            CaptchaType.HCaptcha |
            CaptchaType.GeeTest |
            CaptchaType.CloudflareTurnstile |
            CaptchaType.DataDome;

        SoftId = 80;
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveDataDomeAsync(
        string siteUrl, string captchaUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        // CapMonsterCloud will always use the current Windows OS User-Agent
        // to solve captchas.
        
        if (sessionParams?.Cookies is null)
        {
            throw new ArgumentNullException(
                nameof(sessionParams), "DataDome requires cookies");
        }
        
        if (string.IsNullOrEmpty(sessionParams.UserAgent))
        {
            throw new ArgumentNullException(
                nameof(sessionParams), "DataDome requires a user agent");
        }
        
        sessionParams.Cookies.TryGetValue("datadome", out var datadomeCookie);
        
        if (string.IsNullOrEmpty(datadomeCookie))
        {
            throw new ArgumentException(
                "The cookie must contain a datadome cookie", nameof(sessionParams));
        }

        var content = CreateTaskRequest();

        content.Task = new DataDomeTaskProxyless
        {
            WebsiteURL = siteUrl,
            UserAgent = sessionParams.UserAgent,
            Metadata = new DataDomeMetadata
            {
                CaptchaUrl = captchaUrl,
                DataDomeCookie = $"datadome={datadomeCookie}"
            }
        };
        
        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask", 
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(response, CaptchaType.DataDome,
            cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
        
        // Option 1 (Turnstile)
        if (data is null)
        {
            content.Task = new TurnstileTaskProxyless
            {
                WebsiteKey = siteKey,
                WebsiteUrl = siteUrl,
                PageAction = action,
            };
        }
        
        // Option 2 (CloudFlare token)
        else
        {
            // User-Agent is required
            if (string.IsNullOrEmpty(sessionParams?.UserAgent))
            {
                throw new ArgumentNullException(
                    nameof(sessionParams), "User-Agent is required for Cloudflare challenges");
            }
            
            content.Task = new TurnstileTaskProxyless
            {
                WebsiteKey = siteKey,
                WebsiteUrl = siteUrl,
                UserAgent = sessionParams.UserAgent,
                PageAction = action,
                CData = data,
                PageData = pageData
            };            
        }
            
        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<CloudflareTurnstileResponse>(response, CaptchaType.CloudflareTurnstile,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveCloudflareChallengePageAsync(
        string siteUrl, string pageHtml, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        // Option 3 (CloudFlare cookie)
        if (string.IsNullOrEmpty(sessionParams?.UserAgent))
        {
            throw new ArgumentNullException(
                nameof(sessionParams), "Solving Cloudflare challenges requires a User-Agent");
        }

        if (string.IsNullOrEmpty(sessionParams.Proxy?.Host))
        {
            throw new ArgumentNullException(
                nameof(sessionParams), "Solving Cloudflare challenges requires a proxy");
        }
        
        var content = CreateTaskRequest();
        
        content.Task = new TurnstileTask
        {
            WebsiteUrl = siteUrl,
            WebsiteKey = "n/a", // Not used
            HtmlPageBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(pageHtml)),
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(response, CaptchaType.CloudflareChallengePage,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override StringResponse ParseDataDomeSolution(string taskId, JToken? solution)
    {
        // The solution is like { "domains": { "site.com": { "cookies": { "datadome": "..." } } } }
        // We want to return the datadome cookie. Since site.com varies, we need to
        // take the first domain.
        var cookie = solution
            ?.SelectToken("domains")
            ?.First?.First
            ?.SelectToken("cookies.datadome")
            ?.Value<string>() ?? "";
        
        return new StringResponse { Id = taskId, Response = cookie };
    }
    
    /// <inheritdoc />
    protected override StringResponse ParseCloudflareChallengePageSolution(string taskId, JToken? solution)
    {
        // Get the cf_clearance field from the solution
        var cfClearance = solution?.SelectToken("cf_clearance")?.Value<string>() ?? "";
        
        return new StringResponse { Id = taskId, Response = cfClearance };
    }
}
