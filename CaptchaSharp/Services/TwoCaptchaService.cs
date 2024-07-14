using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.TwoCaptcha;
using System.Collections.Generic;
using System;
using CaptchaSharp.Extensions;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by <c>https://2captcha.com/</c>
/// </summary>
public class TwoCaptchaService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The default <see cref="HttpClient"/> used for requests.
    /// </summary>
    protected readonly HttpClient HttpClient;

    /// <summary>
    /// Set it to false if the service does not support json responses.
    /// </summary>
    public bool UseJsonFlag { get; init; } = true;

    /// <summary>
    /// Will include an Access-Control-Allow-Origin:* header in the response for 
    /// cross-domain AJAX requests in web applications.
    /// </summary>
    public bool AddAcaoHeader { get; set; } = false;

    /// <summary>The ID of the software developer.</summary>
    private const int _softId = 2658;

    /// <summary>
    /// Initializes a <see cref="TwoCaptchaService"/>.</summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public TwoCaptchaService(string apiKey, HttpClient? httpClient = null)
    {
        ApiKey = apiKey;
        this.HttpClient = httpClient ?? new HttpClient();
        
        // TODO: Use https instead of http if possible
        this.HttpClient.BaseAddress = new Uri("http://2captcha.com");
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetStringAsync("res.php",
            new StringPairCollection() 
                .Add("key", ApiKey)
                .Add("action", "getbalance")
                .Add("json", Convert.ToInt32(UseJsonFlag).ToString()),
            cancellationToken);

        if (UseJsonFlag)
        {
            var tcResponse = response.Deserialize<TwoCaptchaResponse>();

            if (tcResponse.IsErrorCode)
            {
                throw new BadAuthenticationException(tcResponse.Request);
            }

            return decimal.Parse(tcResponse.Request, CultureInfo.InvariantCulture);
        }

        if (decimal.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal balance))
        {
            return balance;
        }

        throw new BadAuthenticationException(response);
    }
    #endregion

    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveTextCaptchaAsync(
        string text, TextCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("textcaptcha", text)
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.TextCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<StringResponse>(
                response, CaptchaType.TextCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "base64")
                .Add("body", base64)
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.ImageCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<StringResponse>(
                response, CaptchaType.ImageCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "userrecaptcha")
                .Add("googlekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("data-s", dataS, !string.IsNullOrEmpty(dataS))
                .Add("enterprise", Convert.ToInt32(enterprise).ToString())
                .Add("invisible", Convert.ToInt32(invisible).ToString())
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertProxy(proxy))
                .ToMultipartFormDataContent(),
                cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.ReCaptchaV2,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<StringResponse>(
                response, CaptchaType.ReCaptchaV2,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, bool enterprise = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "userrecaptcha")
                .Add("version", "v3")
                .Add("googlekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("action", action)
                .Add("enterprise", Convert.ToInt32(enterprise).ToString())
                .Add("min_score", minScore.ToString("0.0", CultureInfo.InvariantCulture))
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertProxy(proxy))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.ReCaptchaV3,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<StringResponse>(
                response, CaptchaType.ReCaptchaV3,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "funcaptcha")
                .Add("publickey", publicKey)
                .Add("surl", serviceUrl)
                .Add("pageurl", siteUrl)
                .Add("nojs", Convert.ToInt32(noJs).ToString())
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertProxy(proxy))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.FunCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<StringResponse>(
                response, CaptchaType.FunCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "hcaptcha")
                .Add("sitekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertProxy(proxy))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.HCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<StringResponse>(
                response, CaptchaType.HCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveKeyCaptchaAsync(
        string userId, string sessionId, string webServerSign1, string webServerSign2, string siteUrl,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "keycaptcha")
                .Add("s_s_c_user_id", userId)
                .Add("s_s_c_session_id", sessionId)
                .Add("s_s_c_web_server_sign", webServerSign1)
                .Add("s_s_c_web_server_sign2", webServerSign2)
                .Add("pageurl", siteUrl)
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertProxy(proxy))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.KeyCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<StringResponse>(
                response, CaptchaType.KeyCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "geetest")
                .Add("gt", gt)
                .Add("challenge", challenge)
                .Add("api_server", apiServer!, !string.IsNullOrEmpty(apiServer))
                .Add("pageurl", siteUrl)
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertProxy(proxy))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<GeeTestResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.GeeTest,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<GeeTestResponse>(
                response, CaptchaType.GeeTest,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CapyResponse> SolveCapyAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "capy")
                .Add("captchakey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertProxy(proxy))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<CapyResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.Capy,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<CapyResponse>(
                response, CaptchaType.Capy,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveDataDomeAsync(
        string siteUrl, string captchaUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        // Make sure there is a proxy with a User-Agent
        if (proxy == null || string.IsNullOrEmpty(proxy.Host) || string.IsNullOrEmpty(proxy.UserAgent))
        {
            throw new ArgumentException("A proxy with a User-Agent is required for DataDome captchas.");
        }
            
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "datadome")
                .Add("captcha_url", captchaUrl)
                .Add("pageurl", siteUrl)
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertProxy(proxy))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.DataDome,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<StringResponse>(
                response, CaptchaType.DataDome,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        // Make sure there is a proxy with a User-Agent
        if (proxy == null || string.IsNullOrEmpty(proxy.UserAgent))
        {
            throw new ArgumentException("A User-Agent is required for Cloudflare Turnstile captchas.");
        }
            
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "turnstile")
                .Add("sitekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("action", action ?? string.Empty, !string.IsNullOrEmpty(action))
                .Add("data", data ?? string.Empty, !string.IsNullOrEmpty(data))
                .Add("pagedata", pageData ?? string.Empty, !string.IsNullOrEmpty(pageData))
                .Add("soft_id", _softId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertProxy(proxy))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResult<CloudflareTurnstileResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.CloudflareTurnstile,
                cancellationToken).ConfigureAwait(false)
            : await GetResult<CloudflareTurnstileResponse>(
                response, CaptchaType.CloudflareTurnstile,
                cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Getting the result
    private async Task<T> GetResult<T>(
        TwoCaptchaResponse twoCaptchaResponse, CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (twoCaptchaResponse.IsErrorCode)
        {
            throw new TaskCreationException(twoCaptchaResponse.Request!);
        }

        var task = new CaptchaTask(twoCaptchaResponse.Request!, type);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    internal async Task<T> GetResult<T>(
        string response, CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (IsErrorCode(response))
            throw new TaskCreationException(response);

        var task = new CaptchaTask(TakeSecondSlice(response), type);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <summary></summary>
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await HttpClient.GetStringAsync("res.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", "get")
                .Add("id", task.Id.ToString())
                .Add("json", Convert.ToInt32(UseJsonFlag).ToString()),
            cancellationToken);

        if (response.Contains("CAPCHA_NOT_READY"))
        {
            return null;
        }

        task.Completed = true;

        if (UseJsonFlag)
        {
            if (task.Type == CaptchaType.GeeTest)
            {
                var jObject = JObject.Parse(response);
                var solution = jObject["request"];

                if (solution is null)
                {
                    throw new TaskSolutionException("No solution found");
                }

                if (solution.Type == JTokenType.Object)
                {
                    return response.Deserialize<TwoCaptchaGeeTestResponse>()
                        .Request.ToGeeTestResponse(task.Id) as T;
                }
            }
            else if (task.Type == CaptchaType.Capy)
            {
                var jObject = JObject.Parse(response);
                var solution = jObject["request"];

                if (solution is null)
                {
                    throw new TaskSolutionException("No solution found");
                }
                
                if (solution.Type == JTokenType.Object)
                {
                    return response.Deserialize<TwoCaptchaCapyResponse>()
                        .Request!.ToCapyResponse(task.Id) as T;
                }
            }
            else if (task.Type == CaptchaType.CloudflareTurnstile)
            {
                return response.Deserialize<TwoCaptchaCloudflareTurnstileResponse>()
                    .ToCloudflareTurnstileResponse(task.Id) as T;
            }

            var tcResponse = response.Deserialize<TwoCaptchaResponse>();

            if (tcResponse.IsErrorCode)
            {
                throw new TaskSolutionException(tcResponse.ErrorText!);
            }

            return new StringResponse { Id = task.Id, Response = tcResponse.Request! } as T;
        }

        if (IsErrorCode(response))
        {
            throw new TaskSolutionException(response);
        }

        response = TakeSecondSlice(response);

        return task.Type switch
        {
            CaptchaType.GeeTest => response.Deserialize<GeeTestSolution>().ToGeeTestResponse(task.Id) as T,
            CaptchaType.Capy => response.Deserialize<CapySolution>().ToCapyResponse(task.Id) as T,
            _ => new StringResponse { Id = task.Id, Response = response } as T
        };
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolution(
        long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var action = correct ? "reportgood" : "reportbad";

        var response = await HttpClient.GetStringAsync("res.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", action)
                .Add("id", id.ToString())
                .Add("json", Convert.ToInt32(UseJsonFlag).ToString()),
            cancellationToken);

        if (UseJsonFlag)
        {
            var tcResponse = response.Deserialize<TwoCaptchaResponse>();

            if (tcResponse.IsErrorCode)
                throw new TaskReportException(tcResponse.Request);
        }
        else
        {
            if (IsErrorCode(response))
                throw new TaskReportException(response);
        }
    }
    #endregion

    #region Proxies
    /// <summary></summary>
    protected static IEnumerable<(string, string)> ConvertProxy(Proxy? proxy)
    {
        if (proxy is null)
        {
            return [];
        }
            
        var proxyParams = new List<(string, string)>();
            
        if (proxy.UserAgent is not null)
        {
            proxyParams.Add(("userAgent", proxy.UserAgent));
        }

        if (string.IsNullOrEmpty(proxy.Host))
        {
            return proxyParams;
        }

        proxyParams.AddRange(
        [
            ("proxy", proxy.RequiresAuthentication
                ? $"{proxy.Username}:{proxy.Password}@{proxy.Host}:{proxy.Port}"
                : $"{proxy.Host}:{proxy.Port}"),
            ("proxytype", proxy.Type.ToString())
        ]);

        return proxyParams;
    }
    #endregion

    #region Utility methods
    /// <summary>For non-json response.</summary>
    protected bool IsErrorCode(string response)
    {
        return !response.StartsWith("OK");
    }

    /// <summary>For non-json response.</summary>
    protected static string TakeSecondSlice(string str)
    {
        return str.Split('|')[1];
    }
    #endregion

    #region Capabilities
    /// <summary>
    /// The capabilities of the service.
    /// </summary>
    public new static CaptchaServiceCapabilities Capabilities =>
        CaptchaServiceCapabilities.LanguageGroup |
        CaptchaServiceCapabilities.Language |
        CaptchaServiceCapabilities.Phrases |
        CaptchaServiceCapabilities.CaseSensitivity |
        CaptchaServiceCapabilities.CharacterSets |
        CaptchaServiceCapabilities.Calculations |
        CaptchaServiceCapabilities.MinLength |
        CaptchaServiceCapabilities.MaxLength |
        CaptchaServiceCapabilities.Instructions;

    /// <summary></summary>
    private List<(string, string)> ConvertCapabilities(TextCaptchaOptions? options)
    {
        // If null, don't return any parameters
        if (options is null)
        {
            return [];
        }

        var capabilities = new List<(string, string)>();

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.LanguageGroup))
        {
            capabilities.Add(("language", ((int)options.CaptchaLanguageGroup).ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.Language) &&
            options.CaptchaLanguage != CaptchaLanguage.NotSpecified)
        {
            capabilities.Add(("lang", options.CaptchaLanguage.ToIso6391Code()));
        }

        return capabilities;
    }

    /// <summary></summary>
    protected List<(string, string)> ConvertCapabilities(ImageCaptchaOptions? options)
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
            capabilities.Add(("regsense", Convert.ToInt32(options.CaseSensitive).ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.CharacterSets))
        {
            capabilities.Add(("numeric", ((int)options.CharacterSet).ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.Calculations))
        {
            capabilities.Add(("calc", Convert.ToInt32(options.RequiresCalculation).ToString()));
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

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.LanguageGroup))
        {
            capabilities.Add(("language", ((int)options.CaptchaLanguageGroup).ToString()));
        }

        if (Capabilities.HasFlag(CaptchaServiceCapabilities.Language) &&
            options.CaptchaLanguage != CaptchaLanguage.NotSpecified)
        {
            capabilities.Add(("lang", options.CaptchaLanguage.ToIso6391Code()));
        }

        return capabilities;
    }
    #endregion
}
