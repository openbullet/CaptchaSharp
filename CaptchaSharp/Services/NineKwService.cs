using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models.NineKw;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://www.9kw.eu/
/// </summary>
public class NineKwService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Initializes a <see cref="NineKwService"/>.
    /// </summary>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public NineKwService(string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        ApiKey = apiKey;
        HttpClient.BaseAddress = new Uri("https://www.9kw.eu/");

        // Since this service replies directly with the solution to the task creation request (for image captchas)
        // we need to set a high timeout here, or it will not finish in time
        HttpClient.Timeout = Timeout;
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetJsonAsync<NineKwBalanceResponse>(
            "index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchaguthaben")
                .Add("json", 1),
            cancellationToken)
            .ConfigureAwait(false);

        if (IsError(response))
        {
            throw new BadAuthenticationException(GetErrorMessage(response));   
        }

        return Convert.ToDecimal(response.Credits);
    }
    #endregion

    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveTextCaptchaAsync(
        string text, TextCaptchaOptions? options = default, 
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartAsync<NineKwSubmitResponse>(
            "index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchaupload")
                .Add("file-upload-01", text)
                .Add("textonly", 1)
                .Add("json", 1)
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.TextCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartAsync<NineKwSubmitResponse>(
            "index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchaupload")
                .Add("file-upload-01", base64)
                .Add("base64", 1)
                .Add("json", 1)
                .Add(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        if (IsError(response))
        {
            throw new TaskSolutionException(GetErrorMessage(response));
        }

        return await GetResult<StringResponse>(
            response, CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetJsonAsync<NineKwSubmitResponse>(
            "index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchaupload")
                .Add("interactive", 1)
                .Add("oldsource", "recaptchav2")
                .Add("file-upload-01", siteKey)
                .Add("pageurl", siteUrl)
                .Add("json", 1)
                .Add(ConvertProxy(proxy)),
            cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetJsonAsync<NineKwSubmitResponse>(
            "index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchaupload")
                .Add("interactive", 1)
                .Add("oldsource", "recaptchav3")
                .Add("file-upload-01", siteKey)
                .Add("pageurl", siteUrl)
                .Add("json", 1)
                .Add(ConvertProxy(proxy)),
            cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.ReCaptchaV3, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false,
        string? data = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetJsonAsync<NineKwSubmitResponse>(
            "index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchaupload")
                .Add("interactive", 1)
                .Add("oldsource", "funcaptcha")
                .Add("file-upload-01", publicKey)
                .Add("pageurl", siteUrl)
                .Add("json", 1)
                .Add(ConvertProxy(proxy)),
            cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response, CaptchaType.FunCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetJsonAsync<NineKwSubmitResponse>(
            "index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchaupload")
                .Add("interactive", 1)
                .Add("oldsource", "hcaptcha")
                .Add("file-upload-01", siteKey)
                .Add("pageurl", siteUrl)
                .Add("json", 1)
                .Add(ConvertProxy(proxy)),
            cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response, CaptchaType.HCaptcha, cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region Getting the result
    private async Task<T> GetResult<T>(
        NineKwSubmitResponse response, CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (IsError(response))
        {
            throw new TaskCreationException(GetErrorMessage(response));
        }

        var task = new CaptchaTask(response.CaptchaId, type);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await HttpClient.GetJsonAsync<NineKwCheckResponse>(
            "index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchacorrectdata")
                .Add("id", task.Id)
                .Add("json", 1),
            cancellationToken)
            .ConfigureAwait(false);
        
        // Not solved yet
        if (response.TryAgain is 1)
        {
            return null;
        }

        task.Completed = true;

        if (response.Answer == "ERROR NO USER")
        {
            throw new TaskSolutionException("No workers available");
        }
        
        if (IsError(response))
        {
            throw new TaskSolutionException(GetErrorMessage(response));
        }

        // Only StringResponse is supported
        if (typeof(T) != typeof(StringResponse))
        {
            throw new NotSupportedException();
        }
        
        return new StringResponse { Id = task.Id, Response = response.Answer } as T;
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolution(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetJsonAsync<NineKwResponse>(
            "index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchacorrectback")
                .Add("id", id)
                .Add("correct", correct ? 1 : 2)
                .Add("json", 1),
            cancellationToken).ConfigureAwait(false);
        
        if (IsError(response))
        {
            throw new TaskReportException(GetErrorMessage(response));
        }
    }
    #endregion

    #region Private Methods
    private StringPairCollection GetAuthPair()
        => new StringPairCollection().Add("apikey", ApiKey);

    private static bool IsError(NineKwResponse response)
        => !string.IsNullOrEmpty(response.Error);

    private static string GetErrorMessage(NineKwResponse response)
        => response.Error ?? "Unknown error";
    #endregion

    #region Proxies
    /// <summary></summary>
    private static List<(string, string)> ConvertProxy(Proxy? proxy)
    {
        if (proxy is null)
        {
            return [];
        }

        var proxyParams = new List<(string, string)>();
        
        if (proxy.UserAgent is not null)
        {
            proxyParams.Add(("useragent", proxy.UserAgent));
        }
        
        if (proxy.Cookies is not null)
        {
            proxyParams.Add(("cookies", proxy.GetCookieString()));
        }

        if (!string.IsNullOrEmpty(proxy.Username))
        {
            throw new NotSupportedException(
                "9kw.eu does not support proxies with authentication.");
        }

        if (proxy.Type is not ProxyType.HTTP && 
            proxy.Type is not ProxyType.HTTPS &&
            proxy.Type is not ProxyType.SOCKS5)
        {
            throw new NotSupportedException(
                "9kw.eu only supports HTTP, HTTPS and SOCKS5 proxies.");
        }
        
        if (!string.IsNullOrEmpty(proxy.Host))
        {
            proxyParams.AddRange([
                ("proxy", $"{proxy.Host}:{proxy.Port}"),
                ("proxytype", proxy.Type.ToString().ToLower())
            ]);
        }

        return proxyParams;
    }
    #endregion

    #region Capabilities
    /// <summary>
    /// The capabilities of the service.
    /// </summary>
    public new CaptchaServiceCapabilities Capabilities =>
        CaptchaServiceCapabilities.Phrases |
        CaptchaServiceCapabilities.CaseSensitivity |
        CaptchaServiceCapabilities.CharacterSets |
        CaptchaServiceCapabilities.Calculations |
        CaptchaServiceCapabilities.MinLength |
        CaptchaServiceCapabilities.MaxLength |
        CaptchaServiceCapabilities.Instructions;

    /// <summary></summary>
    private IEnumerable<(string, string)> ConvertCapabilities(ImageCaptchaOptions? options)
    {
        // If null, don't return any parameters
        if (options is null)
        {
            return [];
        }

        var capabilities = new List<(string, string)>();

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.Phrases))
        {
            capabilities.Add(("phrase", Convert.ToInt32(options.IsPhrase).ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.CaseSensitivity))
        {
            capabilities.Add(("case-sensitive", Convert.ToInt32(options.CaseSensitive).ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.CharacterSets))
        {
            var charSet = 0;

            if (options.CharacterSet != CharacterSet.OnlyNumbersOrOnlyLetters)
            {
                charSet = options.CharacterSet switch
                {
                    CharacterSet.OnlyNumbers => 1,
                    CharacterSet.OnlyLetters => 2,
                    CharacterSet.BothNumbersAndLetters => 3,
                    _ => 0
                };
            }

            capabilities.Add(("numeric", charSet.ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.Calculations))
        {
            capabilities.Add(("math", Convert.ToInt32(options.RequiresCalculation).ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.MinLength))
        {
            capabilities.Add(("min_len", options.MinLength.ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.MaxLength))
        {
            capabilities.Add(("max_len", options.MaxLength.ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.Instructions))
        {
            capabilities.Add(("textinstructions", options.TextInstructions));
        }

        return capabilities;
    }
    #endregion
}
