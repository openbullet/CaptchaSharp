using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models.ImageTyperz;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by <c>https://www.imagetyperz.com/</c>
/// </summary>
public class ImageTyperzService : CaptchaService
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
    /// The ID of the software developer.
    /// </summary>
    private const int _affiliateId = 109;

    /// <summary>
    /// Initializes a <see cref="ImageTyperzService"/>.
    /// </summary>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public ImageTyperzService(string apiKey, HttpClient? httpClient = null)
    {
        ApiKey = apiKey;
        this._httpClient = httpClient ?? new HttpClient();
        
        this._httpClient.BaseAddress = new Uri("http://captchatypers.com");

        // Since this service replies directly with the solution to the task creation request (for image captchas)
        // we need to set a high timeout here, or it will not finish in time
        this._httpClient.Timeout = Timeout;
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostToStringAsync(
            "Forms/RequestBalanceToken.ashx",
            GetAuthPair()
                .Add("action", "REQUESTBALANCE"),
            cancellationToken: cancellationToken)
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
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostToStringAsync(
            "Forms/UploadFileAndGetTextNEWToken.ashx",
            GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("file", base64)
                .Add(ConvertCapabilities(options)),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (IsError(response))
        {
            throw new TaskSolutionException(GetErrorMessage(response));
        }

        var split = response.Split(['|'], 2);
        return new StringResponse { Id = split[0], Response = split[1] };
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostToStringAsync(
            enterprise ? "captchaapi/UploadRecaptchaEnt.ashx" : "captchaapi/UploadRecaptchaToken.ashx",
            GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("pageurl", siteUrl)
                .Add("googlekey", siteKey)
                .Add("recaptchatype", invisible ? 2 : 1, !enterprise)
                .Add("enterprise_type", "v2", enterprise)
                .Add("data-s", dataS, !string.IsNullOrEmpty(dataS))
                .Add(GetProxyParams(proxy)),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(response, CaptchaType.ReCaptchaV2, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async
    (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostToStringAsync(
            enterprise ? "captchaapi/UploadRecaptchaEnt.ashx" : "captchaapi/UploadRecaptchaToken.ashx",
            GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("pageurl", siteUrl)
                .Add("googlekey", siteKey)
                .Add("captchaaction", action)
                .Add("score", minScore.ToString("0.0", CultureInfo.InvariantCulture))
                .Add("recaptchatype", 3, !enterprise)
                .Add("enterprise_type", "v3", enterprise)
                .Add(GetProxyParams(proxy)),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response, CaptchaType.ReCaptchaV2, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostToStringAsync(
            "captchaapi/UploadFunCaptcha.ashx",
            GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("captchatype", 13)
                .Add("pageurl", siteUrl)
                .Add("sitekey", publicKey)
                .Add("s_url", serviceUrl)
                .Add(GetProxyParams(proxy)),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response, CaptchaType.FunCaptcha, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostToStringAsync(
            "captchaapi/UploadHCaptchaUser.ashx",
            GetAuthAffiliatePair()
                .Add("captchatype", 11)
                .Add("action", "UPLOADCAPTCHA")
                .Add("pageurl", siteUrl)
                .Add("sitekey", siteKey)
                .Add(GetProxyParams(proxy)),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response, CaptchaType.HCaptcha, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetStringAsync(
            "captchaapi/UploadGeeTestToken.ashx",
            GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("gt", gt)
                .Add("challenge", challenge)
                .Add("api_server", apiServer!, !string.IsNullOrEmpty(apiServer))
                .Add("domain", siteUrl)
                .Add(GetProxyParams(proxy)),
            cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<GeeTestResponse>(
            response, CaptchaType.GeeTest, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<CapyResponse> SolveCapyAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostToStringAsync(
            "captchaapi/UploadCapyCaptchaUser.ashx",
            GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("captchatype", 12)
                .Add("pageurl", siteUrl)
                .Add("sitekey", siteKey)
                .Add(GetProxyParams(proxy)),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<CapyResponse>(
            response, CaptchaType.Capy, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostToStringAsync(
            "captchaapi/Uploadturnstile.ashx",
            GetAuthAffiliatePair()
                .Add("action", "UPLOADCAPTCHA")
                .Add("pageurl", siteUrl)
                .Add("sitekey", siteKey)
                .Add("taction", action!, !string.IsNullOrEmpty(action))
                .Add("data", data!, !string.IsNullOrEmpty(data))
                .Add(GetProxyParams(proxy)),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<CloudflareTurnstileResponse>(
            response, CaptchaType.CloudflareTurnstile, cancellationToken);
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
        
        // If the response starts with a [, it's a JSON array
        if (response.StartsWith('['))
        {
            var responses = response.Deserialize<ImageTyperzTaskCreatedResponse[]>(); 
            response = responses[0].CaptchaId.ToString();
        }

        var task = new CaptchaTask(response, type);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var responseJson = await _httpClient.GetStringAsync(
            "captchaapi/GetCaptchaResponseJson.ashx",
            GetAuthPair()
                .Add("action", "GETTEXT")
                .Add("captchaid", task.Id),
            cancellationToken)
            .ConfigureAwait(false);
        
        if (string.IsNullOrEmpty(responseJson))
        {
            throw new TaskSolutionException("Could not get the solution for the task");
        }
        
        // For some reason, the response is an array with a single element
        var responses = responseJson.Deserialize<ImageTyperzResponse[]>();
        var response = responses[0];

        if (response.Status == "Pending")
        {
            return null;
        }

        task.Completed = true;

        if (response.Status != "Solved")
        {
            throw new TaskSolutionException(response.Error);
        }
        
        // GeeTestResponse needs GeeTest captcha type
        if (typeof(T) == typeof(GeeTestResponse))
        {
            if (task.Type is not CaptchaType.GeeTest)
            {
                throw new TaskSolutionException("The task is not a GeeTest captcha");   
            }

            var geeTestResponse = JObject.Parse(response.Response);
            
            return new GeeTestResponse
            {
                Id = task.Id,
                Challenge = geeTestResponse["geetest_challenge"]!.Value<string>()!,
                Validate = geeTestResponse["geetest_validate"]!.Value<string>()!,
                SecCode = geeTestResponse["geetest_seccode"]!.Value<string>()!
            } as T;
        }
        
        // TODO: Handle Capy response
        
        else if (typeof(T) == typeof(CloudflareTurnstileResponse))
        {
            if (task.Type is not CaptchaType.CloudflareTurnstile)
            {
                throw new TaskSolutionException("The task is not a Cloudflare Turnstile captcha");   
            }
            
            var cloudflareResponse = JObject.Parse(response.Response);
            
            return new CloudflareTurnstileResponse
            {
                Id = task.Id,
                Response = cloudflareResponse["Response"]!.Value<string>()!,
                UserAgent = cloudflareResponse["UserAgent"]!.Value<string>()!
            } as T;
        }

        // If it's not a StringResponse, throw
        if (typeof(T) != typeof(StringResponse))
        {
            throw new NotSupportedException("Only StringResponse and GeeTestResponse are supported");
        }
        
        return new StringResponse { Id = task.Id, Response = response.Response } as T;
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolution(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostToStringAsync(
            "Forms/SetBadImageToken.ashx",
            GetAuthPair()
                .Add("imageid", id)
                .Add("action", "SETBADIMAGE"),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (response != "SUCCESS")
        {
            throw new TaskReportException(response);
        }
    }
    #endregion

    #region Private Methods
    private StringPairCollection GetAuthPair() 
        => new StringPairCollection().Add("token", ApiKey);

    private StringPairCollection GetAuthAffiliatePair()
        => GetAuthPair().Add("affiliateid", _affiliateId);

    private static bool IsError(string response)
        => response.StartsWith("ERROR:");

    private static string GetErrorMessage(string response)
        => response.Replace("ERROR: ", "");

    private static List<(string, string)> GetProxyParams(Proxy? proxy)
    {
        if (proxy == null)
        {
            return [];
        }

        var proxyPairs = new List<(string, string)>();

        if (proxy.UserAgent is not null)
        {
            proxyPairs.Add(("useragent", proxy.UserAgent));
        }

        if (string.IsNullOrEmpty(proxy.Host))
        {
            return proxyPairs;
        }
        
        if (proxy.Type != ProxyType.HTTP && proxy.Type != ProxyType.HTTPS)
        {
            throw new NotSupportedException("The api only supports HTTP proxies");
        }
        
        proxyPairs.AddRange(
        [
            ("proxytype", "HTTP"),
            proxy.RequiresAuthentication
                ? ("proxy", $"{proxy.Host}:{proxy.Port}:{proxy.Username}:{proxy.Password}")
                : ("proxy", $"{proxy.Host}:{proxy.Port}")
        ]);

        return proxyPairs;
    }
    #endregion

    #region Capabilities
    /// <inheritdoc/>
    public override CaptchaServiceCapabilities Capabilities =>
        CaptchaServiceCapabilities.Phrases |
        CaptchaServiceCapabilities.CaseSensitivity |
        CaptchaServiceCapabilities.CharacterSets |
        CaptchaServiceCapabilities.Calculations |
        CaptchaServiceCapabilities.MinLength |
        CaptchaServiceCapabilities.MaxLength;

    private static List<(string, string)> ConvertCapabilities(ImageCaptchaOptions? options)
    {
        // If null, don't return any parameters
        if (options is null)
        {
            return [];
        }

        var capabilities = new List<(string, string)> 
        { 
            ("iscase", options.CaseSensitive.ToString().ToLower()),
            ("isphrase", options.IsPhrase.ToString().ToLower()),
            ("ismath", options.RequiresCalculation.ToString().ToLower()),
            ("minlength", options.MinLength.ToString()),
            ("maxlength", options.MaxLength.ToString())
        };

        var alphanumeric = options.CharacterSet switch
        {
            CharacterSet.OnlyNumbers => 1,
            CharacterSet.OnlyLetters => 2,
            _ => 0
        };

        capabilities.Add(("alphanumeric", alphanumeric.ToString()));

        return capabilities;
    }
    #endregion
}
