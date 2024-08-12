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
using CaptchaSharp.Models.CaptchaOptions;
using CaptchaSharp.Models.CaptchaResponses;

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

    private string AffiliateId { get; set; } = "5e95fff9fe5f8247ff965ac3";
    
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
        if (string.IsNullOrEmpty(base64))
        {
            throw new ArgumentException("The image base64 string is null or empty", nameof(base64));
        }
        
        var payload = new BcsSolveImageRequest
        {
            AccessToken = ApiKey,
            AffiliateId = AffiliateId,
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
        
        return await GetResultAsync<StringResponse>(
            response, CaptchaType.ImageCaptcha,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false,
        bool invisible = false, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var type = invisible ? 2 : 1;
        
        if (enterprise)
        {
            type = 4;
        }

        var payload = new BcsSolveRecaptchaV2Request
        {
            AccessToken = ApiKey,
            AffiliateId = AffiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl,
            Type = type,
            DataS = dataS
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/recaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(
            response, CaptchaType.ReCaptchaV2,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveRecaptchaV3Request
        {
            AccessToken = ApiKey,
            AffiliateId = AffiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl,
            Type = enterprise ? 5 : 3,
            Action = action,
            MinScore = minScore
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/recaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(
            response, CaptchaType.ReCaptchaV3,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false,
        string? data = null, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveFuncaptchaRequest
        {
            AccessToken = ApiKey,
            AffiliateId = AffiliateId,
            SiteKey = publicKey,
            PageUrl = siteUrl,
            SUrl = serviceUrl,
            Data = data,
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/funcaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(
            response, CaptchaType.FunCaptcha,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, bool invisible = false, string? enterprisePayload = null,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveHCaptchaRequest
        {
            AccessToken = ApiKey,
            AffiliateId = AffiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl,
            Invisible = invisible,
            Payload = enterprisePayload
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/hcaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<StringResponse>(
            response, CaptchaType.HCaptcha,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveGeeTestRequest
        {
            AccessToken = ApiKey,
            AffiliateId = AffiliateId,
            Gt = gt,
            Challenge = challenge,
            Domain = siteUrl,
            ApiServer = string.IsNullOrEmpty(apiServer) ? null : apiServer
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/geetest",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<GeeTestResponse>(
            response, CaptchaType.GeeTest,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<CapyResponse> SolveCapyAsync(
        string siteKey, string siteUrl, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveCapyRequest
        {
            AccessToken = ApiKey,
            AffiliateId = AffiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/capy",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<CapyResponse>(
            response, CaptchaType.Capy,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveCloudflareTurnstileRequest
        {
            AccessToken = ApiKey,
            AffiliateId = AffiliateId,
            SiteKey = siteKey,
            PageUrl = siteUrl,
            Action = action,
            CData = data
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/turnstile",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<CloudflareTurnstileResponse>(
            response, CaptchaType.CloudflareTurnstile,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestV4Response> SolveGeeTestV4Async(
        string captchaId, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var payload = new BcsSolveGeeTestV4Request
        {
            AccessToken = ApiKey,
            AffiliateId = AffiliateId,
            CaptchaId = captchaId,
            Domain = siteUrl
        }.WithSessionParams(sessionParams);
        
        var response = await HttpClient.PostJsonAsync<BcsTaskCreatedResponse>(
                "captcha/geetestv4",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResultAsync<GeeTestV4Response>(
            response, CaptchaType.GeeTestV4,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion
    
    #region Getting the result
    private async Task<T> GetResultAsync<T>(
        BcsTaskCreatedResponse response,
        CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (!response.Success)
        {
            throw new TaskCreationException(response.Error!);
        }

        var task = new CaptchaTask(response.Id.ToString(), type);

        return await GetResultAsync<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResultAsync<T>(
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

        if (task.Type is CaptchaType.GeeTestV4)
        {
            var geeTestV4Response = json.Deserialize<BcsSolveGeeTestV4Response>();
            return new GeeTestV4Response
            {
                Id = task.Id,
                CaptchaId = geeTestV4Response.Solution!.CaptchaId,
                LotNumber = geeTestV4Response.Solution!.LotNumber,
                PassToken = geeTestV4Response.Solution!.PassToken,
                GenTime = geeTestV4Response.Solution!.GenTime,
                CaptchaOutput = geeTestV4Response.Solution!.CaptchaOutput
            } as T;
        }

        throw new NotImplementedException();
    }
    #endregion
    
    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolutionAsync(
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
