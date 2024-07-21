using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.BestCaptchaSolver.Requests;
using CaptchaSharp.Models.BestCaptchaSolver.Responses;

namespace CaptchaSharp.Services;

/// <summary>
/// The service offered by https://bestcaptchasolver.com/
/// </summary>
public class BestCaptchaSolverService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    private const string _affiliateId = "5e95fff9fe5f8247ff965ac3";
    
    /// <summary>
    /// Initializes a new <see cref="BestCaptchaSolverService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public BestCaptchaSolverService(string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        ApiKey = apiKey;
        HttpClient.BaseAddress = new Uri("https://bcsapi.xyz/api/");
    }
    
    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetJsonAsync<BcsBalanceResponse>(
            "user/balance",
            new StringPairCollection()
                .Add("access_token", ApiKey),
            cancellationToken)
            .ConfigureAwait(false);

        if (!response.Success)
        {
            throw new BadAuthenticationException(response.Error!);
        }
        
        return decimal.Parse(response.Balance!);
    }
    #endregion
    
    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveImageRequest
        {
            AccessToken = ApiKey,
            AffiliateId = _affiliateId,
            Base64Image = base64,
            CaseSensitive = options?.CaseSensitive,
            IsPhrase = options?.IsPhrase,
            IsMath = options?.RequiresCalculation,
            Alphanumeric = options?.CharacterSet switch
            {
                CharacterSet.OnlyNumbers => 1,
                CharacterSet.OnlyLetters => 2,
                _ => null
            },
            MinLength = options?.MinLength,
            MaxLength = options?.MaxLength
        };

        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/image",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.ImageCaptcha,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false,
        bool invisible = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var type = invisible ? 2 : 1;
        
        if (enterprise)
        {
            type = 4;
        }

        var payload = new BcsSolveRecaptchaV2Request
        {
            AccessToken = ApiKey,
            AffiliateId = _affiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl,
            Type = type,
            DataS = dataS
        };
        
        payload.SetProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/recaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.ReCaptchaV2,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveRecaptchaV3Request
        {
            AccessToken = ApiKey,
            AffiliateId = _affiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl,
            Type = enterprise ? 5 : 3,
            Action = action,
            MinScore = minScore
        };
        
        payload.SetProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/recaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.ReCaptchaV3,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveFuncaptchaRequest
        {
            AccessToken = ApiKey,
            AffiliateId = _affiliateId,
            SiteKey = publicKey,
            PageUrl = siteUrl,
            SUrl = serviceUrl
        };
        
        payload.SetProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/funcaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.FunCaptcha,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveHCaptchaRequest
        {
            AccessToken = ApiKey,
            AffiliateId = _affiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl
        };
        
        payload.SetProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/hcaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.HCaptcha,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveGeeTestRequest
        {
            AccessToken = ApiKey,
            AffiliateId = _affiliateId,
            Gt = gt,
            Challenge = challenge,
            Domain = siteUrl,
            ApiServer = string.IsNullOrEmpty(apiServer) ? null : apiServer
        };
        
        payload.SetProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/geetest",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<GeeTestResponse>(
            response, CaptchaType.GeeTest,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<CapyResponse> SolveCapyAsync(
        string siteKey, string siteUrl, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveCapyRequest
        {
            AccessToken = ApiKey,
            AffiliateId = _affiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl
        };
        
        payload.SetProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/capy",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<CapyResponse>(
            response, CaptchaType.Capy,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveCloudflareTurnstileRequest
        {
            AccessToken = ApiKey,
            AffiliateId = _affiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl,
            Action = action,
            CData = data
        };
        
        payload.SetProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/turnstile",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<CloudflareTurnstileResponse>(
            response, CaptchaType.CloudflareTurnstile,
            cancellationToken: cancellationToken);
    }

    #endregion
    
    #region Getting the result
    private async Task<T> GetResult<T>(
        BcsTaskCreatedResponse response,
        CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (!response.Success)
        {
            throw new TaskCreationException(response.Error!);
        }

        var task = new CaptchaTask(response.Id.ToString(), type);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default) where T : class
    {
        var json = await HttpClient.GetStringAsync(
            $"captcha/{task.Id}",
            new StringPairCollection()
                .Add("access_token", ApiKey),
            cancellationToken)
            .ConfigureAwait(false);

        var response = json.Deserialize<BcsResponse>();

        if (!response.Success)
        {
            throw new TaskSolutionException(response.Error!);
        }
        
        if (response.Status is not "completed")
        {
            return null;
        }
        
        task.Completed = true;
        
        if (task.Type is CaptchaType.ImageCaptcha)
        {
            var imageResponse = json.Deserialize<BcsSolveImageResponse>();
            return new StringResponse
            {
                Id = task.Id,
                Response = imageResponse.Text!
            } as T;
        }

        if (task.Type is CaptchaType.ReCaptchaV2 or CaptchaType.ReCaptchaV3)
        {
            var recaptchaResponse = json.Deserialize<BcsSolveRecaptchaResponse>();
            return new StringResponse
            {
                Id = task.Id,
                Response = recaptchaResponse.GResponse!
            } as T;
        }
        
        if (task.Type is CaptchaType.FunCaptcha)
        {
            var funcaptchaResponse = json.Deserialize<BcsSolveFuncaptchaResponse>();
            return new StringResponse
            {
                Id = task.Id,
                Response = funcaptchaResponse.Solution!
            } as T;
        }
        
        if (task.Type is CaptchaType.HCaptcha)
        {
            var hCaptchaResponse = json.Deserialize<BcsSolveHCaptchaResponse>();
            return new StringResponse
            {
                Id = task.Id,
                Response = hCaptchaResponse.Solution!
            } as T;
        }
        
        if (task.Type is CaptchaType.GeeTest)
        {
            var geeTestResponse = json.Deserialize<BcsSolveGeeTestResponse>();
            return new GeeTestResponse
            {
                Id = task.Id,
                Challenge = geeTestResponse.Solution!.Challenge,
                Validate = geeTestResponse.Solution!.Validate,
                SecCode = geeTestResponse.Solution!.SecCode
            } as T;
        }
        
        if (task.Type is CaptchaType.Capy)
        {
            var capyResponse = json.Deserialize<BcsSolveCapyResponse>();
            return new StringResponse
            {
                Id = task.Id,
                Response = capyResponse.Solution!
            } as T;
        }
        
        if (task.Type is CaptchaType.CloudflareTurnstile)
        {
            var cloudflareResponse = json.Deserialize<BcsSolveCloudflareTurnstileResponse>();
            return new CloudflareTurnstileResponse
            {
                Id = task.Id,
                Response = cloudflareResponse.Solution!,
                UserAgent = cloudflareResponse.UserAgent!
            } as T;
        }

        throw new NotImplementedException();
    }
    #endregion
    
    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolution(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        if (correct)
        {
            throw new ArgumentException(
                "BestCaptchaSolver does not support reporting correct solutions.");
        }
        
        var response = await HttpClient.PostJsonAsync<BcsResponse>(
                $"captcha/bad/{id}",
                new BcsRequest
                {
                    AccessToken = ApiKey,
                },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        if (!response.Success)
        {
            throw new TaskReportException(response.Error!);
        }

        if (response.Status != "updated")
        {
            throw new TaskReportException(response.Status);
        }
    }

    #endregion
}
