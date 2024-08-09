using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.Aycd.Requests;
using CaptchaSharp.Models.Aycd.Requests.RenderParameters;
using CaptchaSharp.Models.Aycd.Responses;
using CaptchaSharp.Models.CaptchaOptions;
using CaptchaSharp.Models.CaptchaResponses;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://aycd.io/
/// </summary>
public class AycdService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The current access token.
    /// </summary>
    private AycdAccessToken? _accessToken;
    
    /// <summary>
    /// Initializes a <see cref="AycdService"/>.
    /// </summary>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public AycdService(string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        ApiKey = apiKey;
        HttpClient.BaseAddress = new Uri("https://autosolve-api.aycd.io/api/v1/");
    }
    
    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);

        // This service does not support getting the balance
        return 999;
    }
    #endregion
    
    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);
        
        var payload = new AycdTaskRequest
        {
            Url = "https://example.com", // Required regardless...
            SiteKey = "n/a", // Required regardless...
            Version = 10,
            RenderParameters = new AycdImageCaptchaRenderParameters
            {
                Base64ImageData = base64,
                CaseSensitive = options?.CaseSensitive ?? false ? "true" : "false",
            }
        };
        
        var response = await HttpClient.PostJsonAsync(
            "tasks/create",
            payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await GetResultAsync<StringResponse>(
            response, payload, CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false,
        bool invisible = false, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);

        var version = invisible ? 1 : 0;
        
        if (enterprise)
        {
            version = 7;
        }
        
        var payload = new AycdTaskRequest
        {
            Url = siteUrl,
            SiteKey = siteKey,
            Version = version
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync(
            "tasks/create",
            payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return await GetResultAsync<StringResponse>(
            response, payload, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);
        
        var payload = new AycdTaskRequest
        {
            Url = siteUrl,
            SiteKey = siteKey,
            Version = enterprise ? 6 : 2,
            Action = action,
            MinScore = minScore
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync(
            "tasks/create",
            payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(
            response, payload, CaptchaType.ReCaptchaV3, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false, string? data = null,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);
        
        var payload = new AycdTaskRequest
        {
            Url = siteUrl,
            SiteKey = publicKey,
            Version = 8,
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync(
            "tasks/create",
            payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(
            response, payload, CaptchaType.FunCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, bool invisible = false, string? enterprisePayload = null,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);
        
        var payload = new AycdTaskRequest
        {
            Url = siteUrl,
            SiteKey = siteKey,
            Version = invisible ? 4 : 3
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync(
            "tasks/create",
            payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(
            response, payload, CaptchaType.HCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);
        
        var payload = new AycdTaskRequest
        {
            Url = siteUrl,
            SiteKey = gt,
            Version = 5,
            RenderParameters = new AycdGeeTestRenderParameters
            {
                Challenge = challenge,
                ApiServer = apiServer ?? "api.geetest.com"
            }
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync(
            "tasks/create",
            payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return await GetResultAsync<GeeTestResponse>(
            response, payload, CaptchaType.GeeTest, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveDataDomeAsync(
        string siteUrl, string captchaUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);
        
        var payload = new AycdTaskRequest
        {
            Url = captchaUrl,
            SiteKey = "none",
            Version = 11
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync(
            "tasks/create",
            payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(
            response, payload, CaptchaType.DataDome, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);
        
        var payload = new AycdTaskRequest
        {
            Url = siteUrl,
            SiteKey = siteKey,
            Version = 12,
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync(
            "tasks/create",
            payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return await GetResultAsync<CloudflareTurnstileResponse>(
            response, payload, CaptchaType.CloudflareTurnstile, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestV4Response> SolveGeeTestV4Async(
        string captchaId, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync(cancellationToken);
        
        var payload = new AycdTaskRequest
        {
            Url = siteUrl,
            SiteKey = captchaId,
            Version = 9
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync(
            "tasks/create",
            payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);
        
        return await GetResultAsync<GeeTestV4Response>(
            response, payload, CaptchaType.GeeTestV4, cancellationToken).ConfigureAwait(false);
    }
    #endregion
    
    #region Getting the result
    private async Task<T> GetResultAsync<T>(
        HttpResponseMessage response, AycdTaskRequest request, CaptchaType type,
        CancellationToken cancellationToken = default) where T : CaptchaResponse
    {
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new TaskCreationException($"{response.StatusCode} - {message}");
        }
        
        var task = new CaptchaTask(request.TaskId, type);

        return await GetResultAsync<T>(task, cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc/>
    protected override async Task<T?> CheckResultAsync<T>(
        CaptchaTask task, CancellationToken cancellationToken = default) where T : class
    {
        await EnsureAccessTokenAsync(cancellationToken);

        // TODO: CARE! Once a task solution is returned once, it is never
        // returned again! So we need some kind of global state to store
        // the task solutions and return them when the task with that id
        // is fetched!
        var response = await HttpClient.GetAsync(
            "tasks",
            cancellationToken).ConfigureAwait(false);
        
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        
        var remoteTasks = json.Deserialize<IEnumerable<AycdTask>>();
        
        var remoteTask = remoteTasks.FirstOrDefault(t => t.TaskId == task.Id);
        
        if (remoteTask is null)
        {
            return null;
        }

        if (remoteTask.Status == "cancelled")
        {
            throw new TaskSolutionException("Task was cancelled");
        }
        
        if (remoteTask.Status == "success")
        {   
            task.Completed = true;
            
            var token = remoteTask.Token;

            if (string.IsNullOrEmpty(token))
            {
                throw new TaskSolutionException("Task was successful but no token was returned");
            }

            if (task.Type == CaptchaType.GeeTest)
            {
                return token.Deserialize<AycdGeeTestSolution>()
                    .ToGeeTestResponse(task.Id) as T;
            }
            
            if (task.Type == CaptchaType.GeeTestV4)
            {
                return token.Deserialize<AycdGeeTestV4Solution>()
                    .ToGeeTestV4Response(task.Id) as T;
            }
            
            return new StringResponse
            {
                Id = task.Id,
                Response = token
            } as T;
        }

        return null;
    }

    #endregion
    
    #region Private Methods
    private async ValueTask EnsureAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (_accessToken is null || _accessToken.ExpiresAt < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            await GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetAsync(
            "https://autosolve-dashboard-api.aycd.io/api/v1/auth/generate-token",
            new StringPairCollection()
                .Add("apiKey", ApiKey),
            cancellationToken).ConfigureAwait(false);
            
        if (!response.IsSuccessStatusCode)
        {
            throw new BadAuthenticationException("Invalid API key");
        }
            
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        _accessToken = json.Deserialize<AycdAccessToken>();
        HttpClient.DefaultRequestHeaders.Add("Authorization",
            $"Token {_accessToken.Token}");
    }
    #endregion
}
