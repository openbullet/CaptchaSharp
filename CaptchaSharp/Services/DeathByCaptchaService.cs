using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.DeathByCaptcha.Tasks;
using CaptchaSharp.Models.DeathByCaptcha.Tasks.Proxied;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models.DeathByCaptcha.Responses;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://www.deathbycaptcha.com/
/// </summary>
public class DeathByCaptchaService : CaptchaService
{
    /// <summary>
    /// Your DeathByCaptcha account name.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Your DeathByCaptcha account password.
    /// </summary>
    public string Password { get; set; }

    /*
     * Sometimes the DBC API randomly replies with query strings even when json is requested, so
     * we will avoid using the Accept: application/json header.
     */

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
    /// Initializes a <see cref="DeathByCaptchaService"/>.
    /// </summary>
    /// <param name="username">Your DeathByCaptcha account name.</param>
    /// <param name="password">Your DeathByCaptcha account password.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> used for requests. If null, a default one will be created.</param>
    public DeathByCaptchaService(string username, string password, HttpClient? httpClient = null) : base(httpClient)
    {
        Username = username;
        Password = password;
        HttpClient.BaseAddress = new Uri("http://api.dbcapi.me/api/");
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsync(
                "user",
                GetAuthPair(),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var query = HttpUtility.ParseQueryString(
            await DecodeIsoResponseAsync(response).ConfigureAwait(false));

        if (IsError(query))
        {
            throw new BadAuthenticationException(GetErrorMessage(query));
        }
        
        var balanceString = query["balance"];
        
        if (balanceString == null)
        {
            throw new TaskCreationException("The server didn't return the balance");
        }

        // The server returns the balance in cents
        return decimal.Parse(balanceString, CultureInfo.InvariantCulture) / 100;
    }
    #endregion

    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostAsync(
            "captcha",
            GetAuthPair()
                .Add("captchafile", $"base64:{base64}")
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(HttpUtility.ParseQueryString(
                await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;

        if (proxy is not null)
        {
            task = new RecaptchaV2DbcTask
            {
                GoogleKey = siteKey,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new RecaptchaV2DbcTaskProxyless
            {
                GoogleKey = siteKey,
                PageUrl = siteUrl
            };
        }

        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 4)
                    .Add("token_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.ReCaptchaV2, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;

        if (proxy is not null)
        {
            task = new RecaptchaV3DbcTask
            {
                GoogleKey = siteKey,
                PageUrl = siteUrl,
                Action = action,
                MinScore = minScore
            }.SetProxy(proxy);
        }
        else
        {
            task = new RecaptchaV3DbcTaskProxyless
            {
                GoogleKey = siteKey,
                PageUrl = siteUrl,
                Action = action,
                MinScore = minScore
            };
        }

        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 5)
                    .Add("token_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.ReCaptchaV3, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false,
        string? data = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;

        if (proxy is not null)
        {
            task = new FunCaptchaDbcTask
            {
                PublicKey = publicKey,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new FunCaptchaDbcTaskProxyless
            {
                PublicKey = publicKey,
                PageUrl = siteUrl
            };
        }

        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 6)
                    .Add("funcaptcha_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.FunCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, bool invisible = false, string? enterprisePayload = null,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;

        if (proxy is not null)
        {
            task = new HCaptchaDbcTask
            {
                SiteKey = siteKey,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new HCaptchaDbcTaskProxyless
            {
                SiteKey = siteKey,
                PageUrl = siteUrl
            };
        }

        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 7)
                    .Add("hcaptcha_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.HCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveKeyCaptchaAsync(
        string userId, string sessionId, string webServerSign1, string webServerSign2, string siteUrl,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;

        if (proxy is not null)
        {
            task = new KeyCaptchaDbcTask
            {
                UserId = userId,
                SessionId = sessionId,
                WebServerSign = webServerSign1,
                WebServerSign2 = webServerSign2,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new KeyCaptchaDbcTaskProxyless
            {
                UserId = userId,
                SessionId = sessionId,
                WebServerSign = webServerSign1,
                WebServerSign2 = webServerSign2,
                PageUrl = siteUrl
            };
        }

        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 10)
                    .Add("keycaptcha_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.KeyCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;
        
        if (proxy is not null)
        {
            task = new GeeTestDbcTask
            {
                Gt = gt,
                Challenge = challenge,
                PageUrl = siteUrl,
            }.SetProxy(proxy);
        }
        else
        {
            task = new GeeTestDbcTaskProxyless
            {
                Gt = gt,
                Challenge = challenge,
                PageUrl = siteUrl,
            };
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 8)
                    .Add("geetest_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<GeeTestResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.GeeTest, cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc/>
    public override async Task<CapyResponse> SolveCapyAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;
        
        if (proxy is not null)
        {
            task = new CapyDbcTask
            {
                CaptchaKey = siteKey,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new CapyDbcTaskProxyless
            {
                CaptchaKey = siteKey,
                PageUrl = siteUrl
            };
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 15)
                    .Add("capy_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<CapyResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.Capy, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveDataDomeAsync(
        string siteUrl, string captchaUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        if (proxy?.Host is null)
        {
            throw new ArgumentNullException(
                nameof(proxy), "DataDome captchas require a proxy");
        }

        // The DBC API will use the User-Agent defined on this page
        // to solve the captcha, so the same one MUST be used to submit
        // the response: https://deathbycaptcha.com/api/datadome
        
        var task = new DataDomeDbcTask
        {
            PageUrl = siteUrl,
            CaptchaUrl = captchaUrl
        }.SetProxy(proxy);
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 21)
                    .Add("datadome_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.DataDome, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        if (proxy?.Host is null)
        {
            throw new ArgumentNullException(
                nameof(proxy), "Cloudflare Turnstile captchas require a proxy");
        }
        
        var task = new CloudflareTurnstileDbcTask
        {
            SiteKey = siteKey,
            PageUrl = siteUrl,
            Action = action,
        }.SetProxy(proxy);
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 12)
                    .Add("turnstile_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<CloudflareTurnstileResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.CloudflareTurnstile, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<LeminCroppedResponse> SolveLeminCroppedAsync(
        string captchaId, string siteUrl, string apiServer = "https://api.leminnow.com/",
        string? divId = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;
        
        if (proxy is not null)
        {
            task = new LeminCroppedDbcTask
            {
                CaptchaId = captchaId,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new LeminCroppedDbcTaskProxyless
            {
                CaptchaId = captchaId,
                PageUrl = siteUrl
            };
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 14)
                    .Add("lemin_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<LeminCroppedResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.LeminCropped, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveAmazonWafAsync(
        string siteKey, string iv, string context, string siteUrl, string? challengeScript = null,
        string? captchaScript = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;
        
        if (proxy is not null)
        {
            task = new AmazonWafDbcTask
            {
                SiteKey = siteKey,
                Iv = iv,
                Context = context,
                PageUrl = siteUrl,
                ChallengeJs = challengeScript,
                CaptchaJs = captchaScript
            }.SetProxy(proxy);
        }
        else
        {
            task = new AmazonWafDbcTaskProxyless
            {
                SiteKey = siteKey,
                Iv = iv,
                Context = context,
                PageUrl = siteUrl,
                ChallengeJs = challengeScript,
                CaptchaJs = captchaScript
            };
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 16)
                    .Add("waf_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.AmazonWaf, cancellationToken).ConfigureAwait(false);
    }
    
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveCyberSiAraAsync(
        string masterUrlId, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        if (proxy?.UserAgent is null)
        {
            throw new ArgumentException("A User-Agent is required for Cyber SiARA captchas.");
        }
        
        DbcTaskProxyless task;
        
        if (proxy.Host is not null)
        {
            task = new CyberSiAraDbcTask
            {
                SlideUrlId = masterUrlId,
                PageUrl = siteUrl,
                UserAgent = proxy.UserAgent
            }.SetProxy(proxy);
        }
        else
        {
            task = new CyberSiAraDbcTaskProxyless
            {
                SlideUrlId = masterUrlId,
                PageUrl = siteUrl,
                UserAgent = proxy.UserAgent
            };
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 17)
                    .Add("siara_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.CyberSiAra, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveMtCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;
        
        if (proxy is not null)
        {
            task = new MtCaptchaDbcTask
            {
                SiteKey = siteKey,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new MtCaptchaDbcTaskProxyless
            {
                SiteKey = siteKey,
                PageUrl = siteUrl
            };
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 18)
                    .Add("mtcaptcha_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.MtCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveCutCaptchaAsync(
        string miseryKey, string apiKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;
        
        if (proxy is not null)
        {
            task = new CutCaptchaDbcTask
            {
                MiseryKey = miseryKey,
                ApiKey = apiKey,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new CutCaptchaDbcTaskProxyless
            {
                MiseryKey = miseryKey,
                ApiKey = apiKey,
                PageUrl = siteUrl
            };
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 19)
                    .Add("cutcaptcha_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.CutCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFriendlyCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;
        
        if (proxy is not null)
        {
            task = new FriendlyCaptchaDbcTask
            {
                SiteKey = siteKey,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new FriendlyCaptchaDbcTaskProxyless
            {
                SiteKey = siteKey,
                PageUrl = siteUrl
            };
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 20)
                    .Add("friendly_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.FriendlyCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveAudioCaptchaAsync(
        string base64, AudioCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var language = options?.CaptchaLanguage ?? CaptchaLanguage.English;
        
        if (!_supportedAudioLanguages.Contains(language))
        {
            throw new ArgumentException("The language is not supported by the service.");
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 13)
                    .Add("audio", base64)
                    .Add("language", language.ToIso6391Code())
                    .ToMultipartFormDataContent(),
                cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.AudioCaptcha, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestV4Response> SolveGeeTestV4Async(
        string captchaId, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        DbcTaskProxyless task;
        
        if (proxy is not null)
        {
            task = new GeeTestV4DbcTask
            {
                CaptchaId = captchaId,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new GeeTestV4DbcTaskProxyless
            {
                CaptchaId = captchaId,
                PageUrl = siteUrl
            };
        }
        
        var response = await HttpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 9)
                    .Add("geetest_params", task.Serialize()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<GeeTestV4Response>(
            HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response).ConfigureAwait(false)),
            CaptchaType.GeeTestV4, cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region Getting the result
    private async Task<T> GetResult<T>(
        NameValueCollection response, CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (IsError(response))
        {
            throw new TaskCreationException(GetErrorMessage(response));
        }
        
        var captchaId = response["captcha"];
        
        if (captchaId == null)
        {
            throw new TaskCreationException("The server didn't return the captcha ID");
        }

        var task = new CaptchaTask(captchaId, type);

        return await GetResultAsync<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResultAsync<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await HttpClient.GetAsync($"captcha/{task.Id}", cancellationToken).ConfigureAwait(false);
        var query = HttpUtility.ParseQueryString(await DecodeIsoResponseAsync(response));

        var text = query["text"];
        
        if (text is null or "")
        {
            return null;
        }

        task.Completed = true;

        if (IsError(query) || query["is_correct"] == "0")
        {
            throw new TaskSolutionException(GetErrorMessage(query));
        }

        if (typeof(T) == typeof(GeeTestResponse))
        {
            var geeTestResponse = text.Deserialize<GeeTestDbcResponse>();
            return new GeeTestResponse
            {
                Id = task.Id,
                Challenge = geeTestResponse.Challenge,
                Validate = geeTestResponse.Validate,
                SecCode = geeTestResponse.Seccode
            } as T;
        }
        
        if (typeof(T) == typeof(CapyResponse))
        {
            var capyResponse = text.Deserialize<CapyDbcResponse>();
            return new CapyResponse
            {
                Id = task.Id,
                CaptchaKey = capyResponse.CaptchaKey,
                ChallengeKey = capyResponse.ChallengeKey,
                Answer = capyResponse.Answer
            } as T;
        }
        
        if (typeof(T) == typeof(LeminCroppedResponse))
        {
            var leminCroppedResponse = text.Deserialize<LeminCroppedDbcResponse>();
            return new LeminCroppedResponse
            {
                Id = task.Id,
                Answer = leminCroppedResponse.Answer,
                ChallengeId = leminCroppedResponse.ChallengeId
            } as T;
        }
        
        if (typeof(T) == typeof(CloudflareTurnstileResponse))
        {
            return new CloudflareTurnstileResponse
            {
                Id = task.Id,
                Response = text,
            } as T;
        }

        if (typeof(T) == typeof(GeeTestV4Response))
        {
            var geeTestV4Response = text.Deserialize<GeeTestV4DbcResponse>();
            return new GeeTestV4Response
            {
                Id = task.Id,
                CaptchaId = geeTestV4Response.CaptchaId,
                LotNumber = geeTestV4Response.LotNumber,
                PassToken = geeTestV4Response.PassToken,
                GenTime = geeTestV4Response.GenTime,
                CaptchaOutput = geeTestV4Response.CaptchaOutput
            } as T;
        }
        
        if (typeof(T) != typeof(StringResponse))
        {
            throw new NotSupportedException();
        }
        
        return new StringResponse { Id = task.Id, Response = text } as T;
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolutionAsync(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        if (correct)
        {
            throw new NotSupportedException("This service doesn't allow reporting of good solutions");
        }

        var response = await HttpClient.PostAsync(
                $"captcha/{id}/report",
                GetAuthPair(),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var query = HttpUtility.ParseQueryString(
            await DecodeIsoResponseAsync(response).ConfigureAwait(false));

        if (IsError(query))
        {
            throw new TaskReportException(GetErrorMessage(query));
        }
    }
    #endregion

    #region Private Methods
    private static async Task<string> DecodeIsoResponseAsync(HttpResponseMessage response)
    {
        using var sr = new StreamReader(
            await response.Content.ReadAsStreamAsync().ConfigureAwait(false),
            Encoding.GetEncoding("iso-8859-1"));
        
        return await sr.ReadToEndAsync();
    }

    private StringPairCollection GetAuthPair()
    {
        return new StringPairCollection()
            .Add("username", Username)
            .Add("password", Password);
    }

    private static bool IsError(NameValueCollection response)
    {
        return response["status"] == "255";
    }

    private static string GetErrorMessage(NameValueCollection response)
    {
        return response["error"] ?? "Unknown error";
    }
    #endregion
}
