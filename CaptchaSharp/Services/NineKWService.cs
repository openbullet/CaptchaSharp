using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by <c>https://www.9kw.eu/</c>
/// </summary>
public class NineKwService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The default <see cref="HttpClient"/> used for requests.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a <see cref="NineKwService"/>.
    /// </summary>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public NineKwService(string apiKey, HttpClient? httpClient = null)
    {
        ApiKey = apiKey;
        this._httpClient = httpClient ?? new HttpClient();
        this._httpClient.BaseAddress = new Uri("https://www.9kw.eu/");

        // Since this service replies directly with the solution to the task creation request (for image captchas)
        // we need to set a high timeout here, or it will not finish in time
        this._httpClient.Timeout = Timeout;
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetStringAsync
            ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaguthaben"),
                cancellationToken)
            .ConfigureAwait(false);

        if (IsError(response))
        {
            throw new BadAuthenticationException(GetErrorMessage(response));   
        }

        return decimal.Parse(response, CultureInfo.InvariantCulture);
    }
    #endregion

    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveTextCaptchaAsync
        (string text, TextCaptchaOptions? options = default, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostMultipartToStringAsync
            ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("file-upload-01", text)
                    .Add("textonly", 1)
                    .ToMultipartFormDataContent(),
                cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            response, CaptchaType.TextCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync
        (string base64, ImageCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostMultipartToStringAsync
            ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("file-upload-01", base64)
                    .Add("base64", 1)
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
        var response = await _httpClient.GetStringAsync
            ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("interactive", 1)
                    .Add("oldsource", "recaptchav2")
                    .Add("file-upload-01", siteKey)
                    .Add("pageurl", siteUrl)
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
        var response = await _httpClient.GetStringAsync
            ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("interactive", 1)
                    .Add("oldsource", "recaptchav3")
                    .Add("file-upload-01", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetStringAsync
            ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("interactive", 1)
                    .Add("oldsource", "funcaptcha")
                    .Add("file-upload-01", publicKey)
                    .Add("pageurl", siteUrl)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync
        (string siteKey, string siteUrl, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetStringAsync
            ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchaupload")
                    .Add("interactive", 1)
                    .Add("oldsource", "hcaptcha")
                    .Add("file-upload-01", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response, CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region Getting the result
    private async Task<T> GetResult<T>(
        string response, CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (IsError(response))
        {
            throw new TaskCreationException(response);
        }

        var task = new CaptchaTask(response, type);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await _httpClient.GetStringAsync
            ("index.cgi",
                GetAuthPair()
                    .Add("action", "usercaptchacorrectdata")
                    .Add("id", task.Id),
                cancellationToken)
            .ConfigureAwait(false);

        // Not solved yet
        if (string.IsNullOrEmpty(response) || response.Contains("CAPTCHA_NOT_READY"))
        {
            return null;
        }

        task.Completed = true;

        if (IsError(response) || response.Contains("ERROR_NO_USER"))
        {
            throw new TaskSolutionException(response);
        }

        // Only StringResponse is supported
        if (typeof(T) != typeof(StringResponse))
        {
            throw new NotSupportedException();
        }
        
        return new StringResponse { Id = task.Id, Response = response } as T;
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolution(
        long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetStringAsync
        ("index.cgi",
            GetAuthPair()
                .Add("action", "usercaptchacorrectback")
                .Add("id", id.ToString())
                .Add("correct", correct ? 1 : 2),
            cancellationToken);

        if (IsError(response))
            throw new TaskReportException(response);
    }
    #endregion

    #region Private Methods
    private StringPairCollection GetAuthPair()
        => new StringPairCollection().Add("apikey", ApiKey);

    private static bool IsError(string response)
        => Regex.IsMatch(response, @"^\d{4} ");

    private static string GetErrorMessage(string response)
        => Regex.Replace(response, @"^\d{4} ", "");
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
        
        // TODO: Check if credentials are supported
        
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
