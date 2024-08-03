using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.EndCaptcha;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://endcaptcha.com/
/// </summary>
public class EndCaptchaService : CaptchaService
{
    /// <summary>
    /// Your username.
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    /// Your password.
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// Initializes a <see cref="EndCaptchaService"/>.
    /// </summary>
    /// <param name="username">The username to use.</param>
    /// <param name="password">The password to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public EndCaptchaService(string username, string password,
        HttpClient? httpClient = null) : base(httpClient)
    {
        Username = username;
        Password = password;
        HttpClient.BaseAddress = new Uri("http://api.endcaptcha.com");
    }
    
    #region Getting the Balance
    /// <inheritdoc />
    public override async Task<decimal> GetBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync(
            "balance",
            new StringPairCollection()
                .Add("username", Username)
                .Add("password", Password)
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        if (response.StartsWith("ERROR:"))
        {
            throw new BadAuthenticationException(response);
        }

        return decimal.Parse(response);
    }
    #endregion
    
    #region Solve Methods
    /// <inheritdoc />
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // It doesn't work when using base64:... as per the docs
        var content = new StringPairCollection()
            .Add("username", Username)
            .Add("password", Password)
            .ToMultipartFormDataContent();
        
        var bytes = Convert.FromBase64String(base64);
        content.Add(new ByteArrayContent(bytes), "image", "image.jpg");
        
        var response = await HttpClient.PostMultipartToStringAsync(
            "upload",
            content,
            cancellationToken)
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
        var tokenParams = new RecaptchaV2TokenParams
        {
            GoogleKey = siteKey,
            PageUrl = siteUrl,
        };
        
        tokenParams.SetProxy(proxy);
        
        var response = await HttpClient.PostMultipartToStringAsync(
            "upload",
            new StringPairCollection()
                .Add("username", Username)
                .Add("password", Password)
                .Add("type", 4)
                .Add("token_params", tokenParams.Serialize())
                .ToMultipartFormDataContent(),
            cancellationToken)
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
        var tokenParams = new RecaptchaV3TokenParams
        {
            GoogleKey = siteKey,
            PageUrl = siteUrl,
            Action = action,
            MinScore = minScore
        };
        
        tokenParams.SetProxy(proxy);
        
        var response = await HttpClient.PostMultipartToStringAsync(
            "upload",
            new StringPairCollection()
                .Add("username", Username)
                .Add("password", Password)
                .Add("type", 5)
                .Add("token_params", tokenParams.Serialize())
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.ReCaptchaV3, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false,
        string? data = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var tokenParams = new FuncaptchaTokenParams
        {
            PublicKey = publicKey,
            PageUrl = siteUrl
        };
        
        tokenParams.SetProxy(proxy);
        
        var response = await HttpClient.PostMultipartToStringAsync(
            "upload",
            new StringPairCollection()
                .Add("username", Username)
                .Add("password", Password)
                .Add("type", 6)
                .Add("funcaptcha_params", tokenParams.Serialize())
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.FunCaptcha, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var tokenParams = new HCaptchaTokenParams
        {
            SiteKey = siteKey,
            PageUrl = siteUrl
        };
        
        tokenParams.SetProxy(proxy);
        
        var response = await HttpClient.PostMultipartToStringAsync(
            "upload",
            new StringPairCollection()
                .Add("username", Username)
                .Add("password", Password)
                .Add("type", 7)
                .Add("hcaptcha_params", tokenParams.Serialize())
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.HCaptcha, cancellationToken)
            .ConfigureAwait(false);
    }
    #endregion
    
    #region Getting the result
    private async Task<T> GetResult<T>(
        string response, CaptchaType type, 
        CancellationToken cancellationToken = default) where T : CaptchaResponse
    {
        if (response.StartsWith("ERROR:"))
        {
            throw new TaskSolutionException(response);
        }
        
        if (response.StartsWith("UNSOLVED_YET:"))
        {
            var task = new CaptchaTask(response.Split('/')[1], type);
            return await GetResult<T>(task, cancellationToken)
                .ConfigureAwait(false);
        }
        
        return (new StringResponse
        {
            Id = "0",
            Response = response
        } as T)!;
    }
    
    /// <inheritdoc />
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default) where T : class
    {
        var response = await HttpClient.PostMultipartToStringAsync(
            $"poll/{task.Id}",
            new StringPairCollection()
                .Add("username", Username)
                .Add("password", Password)
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        if (response.StartsWith("ERROR:"))
        {
            throw new TaskSolutionException(response);
        }
        
        if (response.StartsWith("UNSOLVED_YET"))
        {
            return null;
        }
        
        return new StringResponse
        {
            Id = task.Id,
            Response = response
        } as T;
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc />
    public override async Task ReportSolution(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        // Since the service doesn't always return the captcha id,
        // we can also pass the hash here (future development)
        var response = await HttpClient.PostMultipartToStringAsync(
            "report",
            new StringPairCollection()
                .Add("username", Username)
                .Add("password", Password)
                .Add("captcha_id", id)
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        if (response.StartsWith("ERROR:"))
        {
            throw new TaskReportException(response);
        }
    }
    #endregion
}
