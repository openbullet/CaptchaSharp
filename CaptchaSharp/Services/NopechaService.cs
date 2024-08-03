using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.Nopecha;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://nopecha.com/
/// </summary>
public class NopechaService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }
    
    /// <summary>
    /// Initializes a <see cref="NopechaService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public NopechaService(string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        ApiKey = apiKey;
        HttpClient.BaseAddress = new Uri("https://api.nopecha.com");
    }
    
    #region Getting the Balance
    /// <inheritdoc />
    public override async Task<decimal> GetBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetJsonAsync<NopechaStatusResponse>(
            "status",
            new StringPairCollection()
                .Add("key", ApiKey),
            cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccess)
        {
            throw new BadAuthenticationException(response.Message!);
        }
        
        return Convert.ToDecimal(response.Credit);
    }
    #endregion
    
    #region Solve Methods
    /// <inheritdoc />
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new NopechaSolveImageRequest
        {
            ApiKey = ApiKey,
            ImageData = [base64]
        };

        var response = await HttpClient.PostJsonAsync<NopechaDataResponse>(
            "",
            payload,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
                response, CaptchaType.ImageCaptcha, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false,
        bool invisible = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var payload = new NopechaSolveRecaptchaV2Request
        {
            ApiKey = ApiKey,
            SiteKey = siteKey,
            Url = siteUrl,
            DataS = string.IsNullOrEmpty(dataS) 
                ? null 
                : dataS.Deserialize<Dictionary<string, object>>(),
            Enterprise = enterprise
        };
        
        payload.SetProxy(proxy, siteUrl);
        
        var response = await HttpClient.PostJsonAsync<NopechaDataResponse>(
            "token",
            payload,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
                response, CaptchaType.ReCaptchaV2, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var payload = new NopechaSolveRecaptchaV3Request
        {
            ApiKey = ApiKey,
            SiteKey = siteKey,
            Url = siteUrl,
            DataS = new Dictionary<string, object>
            {
                ["action"] = action,
            },
            Enterprise = enterprise
        };
        
        payload.SetProxy(proxy, siteUrl);
        
        var response = await HttpClient.PostJsonAsync<NopechaDataResponse>(
                "token",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
                response, CaptchaType.ReCaptchaV3, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, bool invisible = false, string? enterprisePayload = null,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var payload = new NopechaSolveHCaptchaRequest
        {
            ApiKey = ApiKey,
            SiteKey = siteKey,
            Url = siteUrl,
            Data = string.IsNullOrEmpty(enterprisePayload)
                ? null 
                : JObject.Parse(enterprisePayload),
        };
        
        payload.SetProxy(proxy, siteUrl);
        
        var response = await HttpClient.PostJsonAsync<NopechaDataResponse>(
                "token",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
                response, CaptchaType.HCaptcha, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var dataDict = new Dictionary<string, object> {};
        
        if (!string.IsNullOrEmpty(action))
        {
            dataDict["action"] = action;
        }
        
        if (!string.IsNullOrEmpty(data))
        {
            dataDict["cdata"] = data;
        }
        
        var payload = new NopechaSolveCloudflareTurnstileRequest
        {
            ApiKey = ApiKey,
            SiteKey = siteKey,
            Url = siteUrl,
            Data = dataDict,
        };
        
        payload.SetProxy(proxy, siteUrl);
        
        var response = await HttpClient.PostJsonAsync<NopechaDataResponse>(
                "token",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<CloudflareTurnstileResponse>(
                response, CaptchaType.CloudflareTurnstile, cancellationToken)
            .ConfigureAwait(false);
    }
    #endregion
    
    #region Getting the result
    private async Task<T> GetResult<T>(
        NopechaDataResponse response, CaptchaType captchaType, CancellationToken cancellationToken)
        where T : CaptchaResponse
    {
        if (!response.IsSuccess)
        {
            throw new TaskCreationException(response.Message!);
        }
        
        var task = new CaptchaTask(response.Data!.ToString(), captchaType);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default) where T : class
    {
        var response = await HttpClient.GetJsonAsync<NopechaDataResponse>(
            "",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("id", task.Id),
            cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccess)
        {
            // Incomplete job
            if (response.Error is 14)
            {
                return null;
            }
            
            throw new TaskSolutionException(response.Message!);
        }

        if (typeof(T) == typeof(CloudflareTurnstileResponse))
        {
            return new CloudflareTurnstileResponse
            {
                Id = task.Id,
                Response = response.Data!.ToString()
            } as T;
        }
        
        if (typeof(T) != typeof(StringResponse))
        {
            throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");
        }
        
        task.Completed = true;
        
        // response.Data can be either a string or an array of strings (with 1 value)
        var result = response.Data!.Type == JTokenType.Array
            ? response.Data!.First!.ToString()
            : response.Data!.ToString();
        
        return new StringResponse
        {
            Id = task.Id,
            Response = result
        } as T;
    }

    #endregion
}
