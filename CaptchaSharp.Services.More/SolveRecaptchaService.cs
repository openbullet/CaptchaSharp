using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;

namespace CaptchaSharp.Services.More;

/// <summary>
/// The service provided by <c>https://solverecaptcha.com/</c>
/// </summary>
public class SolveRecaptchaService : CustomTwoCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="SolveRecaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public SolveRecaptchaService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("http://api.solverecaptcha.com"), httpClient)
    {
        this.HttpClient.DefaultRequestHeaders.Host = "api.solverecaptcha.com";
        this.HttpClient.Timeout = Timeout;

        SupportedCaptchaTypes =
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3;
    }

    /// <inheritdoc/>
    public override Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        // There is no balance since this service has a monthly subscription
        return Task.FromResult(999m);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async
    (string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetStringAsync
            ("",
                GetAuthPair()
                    .Add("sitekey", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add("version", invisible ? "invisible" : "v2")
                    .Add("invisible", Convert.ToInt32(invisible).ToString())
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
            .ConfigureAwait(false);

        if (IsErrorCode(response))
            ThrowException(response);

        return new StringResponse { Id = 0, Response = TakeSecondSlice(response) };
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async
    (string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, bool enterprise = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetStringAsync
            ("",
                GetAuthPair()
                    .Add("sitekey", siteKey)
                    .Add("pageurl", siteUrl)
                    .Add("action", action)
                    .Add("min_score", minScore.ToString("0.0", CultureInfo.InvariantCulture))
                    .Add("version", "v3")
                    .Add(ConvertProxy(proxy)),
                cancellationToken)
            .ConfigureAwait(false);

        if (IsErrorCode(response))
            ThrowException(response);

        return new StringResponse { Id = 0, Response = TakeSecondSlice(response) };
    }

    /// <inheritdoc/>
    public override Task ReportSolution
        (long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    private StringPairCollection GetAuthPair()
        => new StringPairCollection().Add("key", ApiKey);

    private void ThrowException(string response)
    {
        throw response switch
        {
            "ERROR_API_KEY_NOT_FOUND" or "ERROR_ACCESS_DENIED" => new BadAuthenticationException(response),
            "ERROR_NO_AVAILABLE_THREADS" => new TaskCreationException(response),
            "ERROR_CAPTCHA_UNSOLVABLE" => new TaskSolutionException(response),
            _ => new Exception(response)
        };
    }
}
