using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.DeCaptcher;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by <c>https://de-captcher.com/</c>
/// </summary>
public class DeCaptcherService : CaptchaService
{
    /// <summary>
    /// Your DeCaptcher account name.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Your DeCaptcher account password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>The default <see cref="HttpClient"/> used for requests.</summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a <see cref="DeCaptcherService"/>.
    /// </summary>
    /// 
    public DeCaptcherService(string username, string password, HttpClient? httpClient = null)
    {
        Username = username;
        Password = password;
        this._httpClient = httpClient ?? new HttpClient();
        
        // TODO: Use https instead of http if possible
        this._httpClient.BaseAddress = new Uri("http://poster.de-captcher.com/");
            
        // Since this service replies directly with the solution to the task creation request
        // we need to set a high timeout here, or it will not finish in time
        this._httpClient.Timeout = Timeout;
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostMultipartToStringAsync("",
                GetAuthPair().Add("function", "balance").ToMultipartFormDataContent(), cancellationToken)
            .ConfigureAwait(false);

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
        var response = await _httpClient.PostMultipartToStringAsync("",
                GetAuthPair()
                    .Add("function", "picture2")
                    .Add("pict", text)
                    .Add("pict_type", 83)
                    .Add("lang", options?.CaptchaLanguage.ToIso6391Code() ?? string.Empty, options is not null)
                    .ToMultipartFormDataContent(),
                cancellationToken)
            .ConfigureAwait(false);

        if (DeCaptcherResponse.TryParse(response, out var resp))
        {
            return new StringResponse { Id = GetCaptchaId(resp), Response = resp.Text };
        }

        throw new TaskSolutionException(response);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        var content = GetAuthPair()
            .Add("function", "picture2")
            .Add("pict_type", 0)
            .Add("lang", options?.CaptchaLanguage.ToIso6391Code() ?? string.Empty, options is not null)
            .ToMultipartFormDataContent();

        var buffer = Convert.FromBase64String(base64);
        content.Add(new ByteArrayContent(buffer), "pict");

        var response = await _httpClient.PostMultipartToStringAsync("",
                content,
                cancellationToken)
            .ConfigureAwait(false);

        if (DeCaptcherResponse.TryParse(response, out var resp))
        {
            return new StringResponse { Id = GetCaptchaId(resp), Response = resp.Text };
        }

        throw new TaskSolutionException(response);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        if (proxy is not null)
        {
            if (proxy.RequiresAuthentication)
            {
                throw new NotSupportedException("Authenticated proxies are not supported");
            }

            if (proxy.Type != ProxyType.SOCKS4 && proxy.Type != ProxyType.SOCKS5)
            {
                throw new NotSupportedException("Only SOCKS proxies are supported");
            }

            if (siteUrl.StartsWith("https"))
            {
                throw new NotSupportedException("Only http sites are supported");
            }
        }

        var pairs = GetAuthPair()
            .Add("function", "proxy_url")
            .Add("url", siteUrl)
            .Add("key", siteKey);

        if (proxy != null)
        {
            pairs.Add("proxy", $"{proxy.Host}:{proxy.Port}");
        }

        var response = await _httpClient.PostMultipartToStringAsync("",
                pairs.ToMultipartFormDataContent(),
                cancellationToken)
            .ConfigureAwait(false);

        if (DeCaptcherResponse.TryParse(response, out DeCaptcherResponse resp))
        {
            return new StringResponse { Id = GetCaptchaId(resp), Response = resp.Text };
        }

        throw new TaskSolutionException(response);
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolution(
        long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var (major, minor) = LongToInts(id);

        await _httpClient.PostMultipartToStringAsync("",
                GetAuthPair()
                    .Add("function", "picture_bad2")
                    .Add("major_id", major)
                    .Add("minor_id", minor)
                    .ToMultipartFormDataContent(), cancellationToken)
            .ConfigureAwait(false);

        // TODO: Find a way to check if the api accepted the report or not
    }
    #endregion

    #region Private Methods
    private StringPairCollection GetAuthPair()
    {
        return new StringPairCollection()
            .Add("username", Username)
            .Add("password", Password);
    }

    private long GetCaptchaId(DeCaptcherResponse response)
    {
        return IntsToLong(response.MajorID, response.MinorID);
    }

    // Encodes two 32-bit integers as a 64-bit long
    private static long IntsToLong(int a, int b)
    {
        long l = b;
        l <<= 32;
        l |= (uint)a;
        return l;
    }

    // Gets two 32-bit integers by splitting a 64-bit long
    private static (int, int) LongToInts(long longId)
    {
        return ((int)(longId & uint.MaxValue), (int)(longId >> 32));
    }
    #endregion
}
