using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.DeathByCaptcha.Tasks;
using CaptchaSharp.Services.DeathByCaptcha.Tasks.Proxied;
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

    /// <summary>
    /// The default <see cref="HttpClient"/> used for requests.
    /// </summary>
    private readonly HttpClient _httpClient;

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
    public DeathByCaptchaService(string username, string password, HttpClient? httpClient = null)
    {
        Username = username;
        Password = password;
        this._httpClient = httpClient ?? new HttpClient();
        
        // TODO: Use https instead of http if possible
        this._httpClient.BaseAddress = new Uri("http://api.dbcapi.me/api/");
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync(
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
        var response = await _httpClient.PostAsync
        ("captcha",
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
        DBCTaskProxyless task;

        if (proxy is not null)
        {
            task = new RecaptchaV2Task
            {
                GoogleKey = siteKey,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new RecaptchaV2TaskProxyless
            {
                GoogleKey = siteKey,
                PageUrl = siteUrl
            };
        }

        var response = await _httpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 4)
                    .Add("token_params", task.SerializeLowerCase()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.ReCaptchaV2, cancellationToken);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action, float minScore, bool enterprise = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        DBCTaskProxyless task;

        if (proxy is not null)
        {
            task = new RecaptchaV3Task
            {
                GoogleKey = siteKey,
                PageUrl = siteUrl,
                Action = action,
                Min_Score = minScore
            }.SetProxy(proxy);
        }
        else
        {
            task = new RecaptchaV3TaskProxyless
            {
                GoogleKey = siteKey,
                PageUrl = siteUrl,
                Action = action,
                Min_Score = minScore
            };
        }

        var response = await _httpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 5)
                    .Add("token_params", task.SerializeLowerCase()),
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
        DBCTaskProxyless task;

        if (proxy is not null)
        {
            task = new FuncaptchaTask
            {
                PublicKey = publicKey,
                PageUrl = siteUrl
            }.SetProxy(proxy);
        }
        else
        {
            task = new FuncaptchaTaskProxyless
            {
                PublicKey = publicKey,
                PageUrl = siteUrl
            };
        }

        var response = await _httpClient.PostAsync(
                "captcha",
                GetAuthPair()
                    .Add("type", 6)
                    .Add("funcaptcha_params", task.SerializeLowerCase()),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            HttpUtility.ParseQueryString(await DecodeIsoResponse(response)),
            CaptchaType.FunCaptcha, cancellationToken);
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
        var response = await _httpClient.GetAsync($"captcha/{task.Id}", cancellationToken);
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
        
        // Only StringResponse is supported
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
        long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        if (correct)
            throw new NotSupportedException("This service doesn't allow reporting of good solutions");

        var response = await _httpClient.PostAsync(
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
