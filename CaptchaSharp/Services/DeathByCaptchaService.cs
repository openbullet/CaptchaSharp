using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.DeathByCaptcha.Tasks;
using CaptchaSharp.Models.DeathByCaptcha.Tasks.Proxied;
using System;
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
/// The service provided by <c>https://www.deathbycaptcha.com/</c>
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

        var query = HttpUtility.ParseQueryString(await DecodeIsoResponse(response));

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
            cancellationToken);

        return await GetResult<StringResponse>(HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.ImageCaptcha, cancellationToken);
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
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.ReCaptchaV2, cancellationToken);
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
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.ReCaptchaV3, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
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
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.FunCaptcha, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
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
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.HCaptcha, cancellationToken);
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
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.KeyCaptcha, cancellationToken);
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
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.GeeTest, cancellationToken);
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
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.Capy, cancellationToken);
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
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.DataDome, cancellationToken);
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
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.CloudflareTurnstile, cancellationToken);
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

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await HttpClient.GetAsync($"captcha/{task.Id}", cancellationToken);
        var query = HttpUtility.ParseQueryString(await DecodeIsoResponse(response));

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
        
        if (typeof(T) == typeof(CloudflareTurnstileResponse))
        {
            return new CloudflareTurnstileResponse
            {
                Id = task.Id,
                Response = text,
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
    public override async Task ReportSolution(
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

        var query = HttpUtility.ParseQueryString(await DecodeIsoResponse(response));

        if (IsError(query))
            throw new TaskReportException(GetErrorMessage(query));
    }
    #endregion

    #region Private Methods
    private async Task<string> DecodeIsoResponse(HttpResponseMessage response)
    {
        using var sr = new StreamReader(
            await response.Content.ReadAsStreamAsync(),
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
