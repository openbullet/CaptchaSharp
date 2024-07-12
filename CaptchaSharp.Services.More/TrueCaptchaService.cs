using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;

namespace CaptchaSharp.Services.More;

/// <summary>
/// The service provided by <c>https://apitruecaptcha.org/</c>
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
    /// The default <see cref="HttpClient"/> used for requests.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a <see cref="TrueCaptchaService"/>.
    /// </summary>
    /// <param name="userId">Your user id.</param>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public TrueCaptchaService(string userId, string apiKey, HttpClient? httpClient = null)
    {
        UserId = userId;
        ApiKey = apiKey;

        this._httpClient = httpClient ?? new HttpClient();
        this._httpClient.BaseAddress = new Uri("https://api.apitruecaptcha.org/");
        this._httpClient.Timeout = Timeout;
    }

    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetStringAsync
            ("one/getbalance",
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
        var content = new JObject();
        content.Add("userid", UserId);
        content.Add("apikey", ApiKey);
        content.Add("data", base64);

        var response = await _httpClient.PostJsonToStringAsync
            ("one/gettext",
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
            
        return new StringResponse { Id = 0, Response = result.ToString() };
    }
}
