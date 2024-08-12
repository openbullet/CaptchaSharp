using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.TwoCaptcha;
using System.Collections.Generic;
using System;
using System.Collections.Immutable;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models.CaptchaOptions;
using CaptchaSharp.Models.CaptchaResponses;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://2captcha.com/
/// </summary>
public class TwoCaptchaService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

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
    private int SoftId { get; set; } = 2658;
    
    private readonly ImmutableList<CaptchaLanguage> _supportedAudioLanguages = new List<CaptchaLanguage>()
    {
        CaptchaLanguage.English,
        CaptchaLanguage.French,
        CaptchaLanguage.German,
        CaptchaLanguage.Greek,
        CaptchaLanguage.Portuguese,
        CaptchaLanguage.Russian
    }.ToImmutableList();

    /// <summary>
    /// Initializes a <see cref="TwoCaptchaService"/>.</summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public TwoCaptchaService(string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        ApiKey = apiKey;
        HttpClient.BaseAddress = new Uri("http://2captcha.com");
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
            cancellationToken)
            .ConfigureAwait(false);

        if (UseJsonFlag)
        {
            var tcResponse = response.Deserialize<TwoCaptchaResponse>();

            if (tcResponse.IsErrorCode)
            {
                throw new BadAuthenticationException(tcResponse.Request!);
            }

            return decimal.Parse(tcResponse.Request!, CultureInfo.InvariantCulture);
        }

        if (decimal.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out var balance))
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
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.TextCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.TextCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(base64))
        {
            throw new ArgumentException("The image base64 string is null or empty", nameof(base64));
        }
        
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "base64")
                .Add("body", base64)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.ImageCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.ImageCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "userrecaptcha")
                .Add("googlekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("data-s", dataS)
                .Add("enterprise", Convert.ToInt32(enterprise).ToString())
                .Add("invisible", Convert.ToInt32(invisible).ToString())
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
                cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.ReCaptchaV2,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.ReCaptchaV2,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, bool enterprise = false,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
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
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.ReCaptchaV3,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.ReCaptchaV3,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false,
        string? data = null, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var pairs = new StringPairCollection()
            .Add("key", ApiKey)
            .Add("method", "funcaptcha")
            .Add("publickey", publicKey)
            .Add("surl", serviceUrl)
            .Add("pageurl", siteUrl)
            .Add("nojs", Convert.ToInt32(noJs).ToString())
            .Add("soft_id", SoftId)
            .Add("json", "1", UseJsonFlag)
            .Add("header_acao", "1", AddAcaoHeader)
            .Add(ConvertSessionParams(sessionParams));
        
        // If data is not null and is a JSON object, set
        // data[key] = value in the request for each key-value pair
        if (!string.IsNullOrEmpty(data) && data.StartsWith('{') && data.EndsWith('}'))
        {
            var jObject = JObject.Parse(data);
            foreach (var property in jObject.Properties())
            {
                pairs.Add($"data[{property.Name}]", property.Value.ToString());
            }
        }
        
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            pairs.ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.FunCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.FunCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, bool invisible = false, string? enterprisePayload = null,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "hcaptcha")
                .Add("sitekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("invisible", Convert.ToInt32(invisible).ToString())
                .Add("data", enterprisePayload)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.HCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.HCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveKeyCaptchaAsync(
        string userId, string sessionId, string webServerSign1, string webServerSign2, string siteUrl,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
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
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.KeyCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.KeyCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "geetest")
                .Add("gt", gt)
                .Add("challenge", challenge)
                .Add("api_server", apiServer)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<GeeTestResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.GeeTest,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<GeeTestResponse>(
                response, CaptchaType.GeeTest,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CapyResponse> SolveCapyAsync(
        string siteKey, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "capy")
                .Add("captchakey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<CapyResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.Capy,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<CapyResponse>(
                response, CaptchaType.Capy,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveDataDomeAsync(
        string siteUrl, string captchaUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        // Make sure there is a proxy with a User-Agent
        if (string.IsNullOrEmpty(sessionParams?.UserAgent) || sessionParams.Proxy is null)
        {
            throw new ArgumentException("A proxy with a User-Agent is required for DataDome captchas.");
        }
            
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "datadome")
                .Add("captcha_url", captchaUrl)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.DataDome,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.DataDome,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        // Make sure there is a proxy with a User-Agent
        if (string.IsNullOrEmpty(sessionParams?.UserAgent))
        {
            throw new ArgumentException("A User-Agent is required for Cloudflare Turnstile captchas.");
        }
            
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "turnstile")
                .Add("sitekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("action", action)
                .Add("data", data)
                .Add("pagedata", pageData)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<CloudflareTurnstileResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.CloudflareTurnstile,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<CloudflareTurnstileResponse>(
                response, CaptchaType.CloudflareTurnstile,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<LeminCroppedResponse> SolveLeminCroppedAsync(
        string captchaId, string siteUrl, string apiServer = "https://api.leminnow.com/",
        string? divId = null, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "lemin")
                .Add("captcha_id", captchaId)
                .Add("pageurl", siteUrl)
                .Add("api_server", apiServer)
                .Add("div_id", divId)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<LeminCroppedResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.LeminCropped,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<LeminCroppedResponse>(
                response, CaptchaType.LeminCropped,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveAmazonWafAsync(
        string siteKey, string iv, string context, string siteUrl, string? challengeScript = null,
        string? captchaScript = null, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "amazon_waf")
                .Add("sitekey", siteKey)
                .Add("iv", iv)
                .Add("context", context)
                .Add("pageurl", siteUrl)
                .Add("challenge_script", challengeScript)
                .Add("captcha_script", captchaScript)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.AmazonWaf,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.AmazonWaf,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveCyberSiAraAsync(
        string masterUrlId, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        if (sessionParams?.UserAgent is null)
        {
            throw new ArgumentException("A User-Agent is required for Cyber SiARA captchas.");
        }
        
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "cybersiara")
                .Add("master_url_id", masterUrlId)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.CyberSiAra,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.CyberSiAra,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveMtCaptchaAsync(
        string siteKey, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "mt_captcha")
                .Add("sitekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.MtCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.MtCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveCutCaptchaAsync(
        string miseryKey, string apiKey, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "cutcaptcha")
                .Add("misery_key", miseryKey)
                .Add("api_key", apiKey)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.CutCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.CutCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFriendlyCaptchaAsync(
        string siteKey, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "friendly_captcha")
                .Add("sitekey", siteKey)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.FriendlyCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.FriendlyCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveAtbCaptchaAsync(
        string appId, string apiServer, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "atb_captcha")
                .Add("app_id", appId)
                .Add("api_server", apiServer)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.AtbCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.AtbCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<TencentCaptchaResponse> SolveTencentCaptchaAsync(
        string appId, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "tencent")
                .Add("app_id", appId)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return UseJsonFlag
            ? await GetResultAsync<TencentCaptchaResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.TencentCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<TencentCaptchaResponse>(
                response, CaptchaType.TencentCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveAudioCaptchaAsync(
        string base64, AudioCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "audio")
                .Add("body", base64)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertCapabilities(options))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.AudioCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.AudioCaptcha,
                cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestV4Response> SolveGeeTestV4Async(
        string captchaId, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("method", "geetest_v4")
                .Add("captcha_id", captchaId)
                .Add("pageurl", siteUrl)
                .Add("soft_id", SoftId)
                .Add("json", "1", UseJsonFlag)
                .Add("header_acao", "1", AddAcaoHeader)
                .Add(ConvertSessionParams(sessionParams))
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        return UseJsonFlag
            ? await GetResultAsync<GeeTestV4Response>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.GeeTestV4,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<GeeTestV4Response>(
                response, CaptchaType.GeeTestV4,
                cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region Getting the result
    private async Task<T> GetResultAsync<T>(
        TwoCaptchaResponse twoCaptchaResponse, CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (twoCaptchaResponse.IsErrorCode)
        {
            throw new TaskCreationException(twoCaptchaResponse.GetErrorMessage());
        }

        var task = new CaptchaTask(twoCaptchaResponse.Request!, type);

        return await GetResultAsync<T>(task, cancellationToken).ConfigureAwait(false);
    }

    internal async Task<T> GetResultAsync<T>(
        string response, CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (IsErrorCode(response))
        {
            throw new TaskCreationException(response);
        }

        var task = new CaptchaTask(TakeSecondSlice(response), type);

        return await GetResultAsync<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <summary></summary>
    protected override async Task<T?> CheckResultAsync<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await HttpClient.GetStringAsync("res.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", "get")
                .Add("id", task.Id)
                .Add("json", Convert.ToInt32(UseJsonFlag).ToString()),
            cancellationToken).ConfigureAwait(false);

        if (response.Contains("CAPCHA_NOT_READY"))
        {
            return null;
        }

        task.Completed = true;

        try
        {
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
                            .Request?.ToGeeTestResponse(task.Id) as T;
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
                else if (task.Type == CaptchaType.LeminCropped)
                {
                    return response.Deserialize<TwoCaptchaLeminCroppedResponse>()
                        .Request!.ToLeminCroppedResponse(task.Id) as T;
                }
                else if (task.Type == CaptchaType.AmazonWaf)
                {
                    return response.Deserialize<TwoCaptchaAmazonWafResponse>()
                        .Request!.ToStringResponse(task.Id) as T;
                }
                else if (task.Type == CaptchaType.TencentCaptcha) 
                {
                    return response.Deserialize<TwoCaptchaTencentCaptchaResponse>()
                        .Request!.ToTencentCaptchaResponse(task.Id) as T;
                }
                else if (task.Type == CaptchaType.GeeTestV4)
                {
                    return response.Deserialize<TwoCaptchaGeeTestV4Response>()
                        .Request!.ToGeeTestV4Response(task.Id) as T;
                }

                var tcResponse = response.Deserialize<TwoCaptchaResponse>();

                if (tcResponse.IsErrorCode)
                {
                    throw new TaskSolutionException(tcResponse.GetErrorMessage());
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
        catch (Exception ex)
        {
            throw new TaskSolutionException(response, ex);
        }
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolutionAsync(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var action = correct ? "reportgood" : "reportbad";

        var response = await HttpClient.GetStringAsync("res.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", action)
                .Add("id", id)
                .Add("json", Convert.ToInt32(UseJsonFlag).ToString()),
            cancellationToken).ConfigureAwait(false);

        if (UseJsonFlag)
        {
            var tcResponse = response.Deserialize<TwoCaptchaResponse>();

            if (tcResponse.IsErrorCode)
            {
                throw new TaskReportException(tcResponse.Request!);
            }
        }
        else
        {
            if (IsErrorCode(response))
            {
                throw new TaskReportException(response);
            }
        }
    }
    #endregion

    #region Proxies
    /// <summary></summary>
    protected static IEnumerable<(string, string)> ConvertSessionParams(
        SessionParams? sessionParams)
    {
        if (sessionParams is null)
        {
            return [];
        }
            
        var pairs = new List<(string, string)>();
            
        if (sessionParams.UserAgent is not null)
        {
            pairs.Add(("userAgent", sessionParams.UserAgent));
        }
        
        var proxy = sessionParams.Proxy;

        if (proxy is null)
        {
            return pairs;
        }

        pairs.AddRange(
        [
            ("proxy", proxy.RequiresAuthentication
                ? $"{proxy.Username}:{proxy.Password}@{proxy.Host}:{proxy.Port}"
                : $"{proxy.Host}:{proxy.Port}"),
            ("proxytype", proxy.Type.ToString())
        ]);

        return pairs;
    }
    #endregion

    #region Utility methods
    /// <summary>For non-json response.</summary>
    protected bool IsErrorCode(string response)
    {
        return !response.StartsWith("OK") ||
               response.Contains("ERROR", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>For non-json response.</summary>
    protected static string TakeSecondSlice(string str)
    {
        return str.Split('|')[1].Replace("\r\n", "").Trim();
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
    
    /// <summary></summary>
    protected List<(string, string)> ConvertCapabilities(AudioCaptchaOptions? options)
    {
        var language = options?.CaptchaLanguage ?? CaptchaLanguage.English;
        
        if (!_supportedAudioLanguages.Contains(language))
        {
            throw new ArgumentException("The language is not supported by the service.");
        }
        
        return [("lang", language.ToIso6391Code())];
    }
    #endregion
}
