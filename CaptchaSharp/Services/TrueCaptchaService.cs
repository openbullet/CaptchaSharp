using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Extensions;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://apitruecaptcha.org/
/// </summary>
public class TrueCaptchaService : CaptchaService
{
    /// <summary>
    /// Your user id.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Initializes a <see cref="TrueCaptchaService"/>.
    /// </summary>
    /// <param name="userId">Your user id.</param>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public TrueCaptchaService(string userId, string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        UserId = userId;
        ApiKey = apiKey;

        this.HttpClient.BaseAddress = new Uri("https://api.apitruecaptcha.org/");
        this.HttpClient.Timeout = Timeout;
    }

    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetStringAsync(
            "one/getbalance",
            new StringPairCollection()
                .Add("username", UserId)
                .Add("apikey", ApiKey),
            cancellationToken)
            .ConfigureAwait(false);

        if (decimal.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out var balance))
        {
            return balance;
        }

        throw new BadAuthenticationException(response);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync
        (string base64, ImageCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        var content = new JObject
        {
            { "userid", UserId },
            { "apikey", ApiKey },
            { "data", base64 }
        };

        var response = await HttpClient.PostJsonToStringAsync(
            "one/gettext",
            content,
            camelizeKeys: false,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var jObject = JObject.Parse(response);
        var result = jObject["result"];

        if (result is null)
        {
            throw new TaskSolutionException(response);
        }
        
        var requestId = jObject["requestId"]!.Value<string>()!;
            
        return new StringResponse { Id = requestId, Response = result.ToString() };
    }
    
    /// <inheritdoc/>
    public override async Task ReportSolutionAsync(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var content = new JObject
        {
            { "userid", UserId },
            { "apikey", ApiKey },
            { "request_id", id }
        };

        var response = await HttpClient.PostJsonToStringAsync(
                "one/report_error",
                content,
                camelizeKeys: false,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var jObject = JObject.Parse(response);
        var result = jObject["result"];

        if (result is null || result.Value<string>() != "True")
        {
            throw new TaskReportException(response);
        }
    }
}
