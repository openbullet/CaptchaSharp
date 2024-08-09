using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models.CaptchaOptions;
using CaptchaSharp.Models.CaptchaResponses;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by the CapMonster OCR application by ZennoLab.
/// </summary>
public class CapMonsterService : CustomTwoCaptchaService
{
    /// <summary>Initializes a <see cref="CapMonsterService"/>.</summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="baseUri">The base URI of the service.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public CapMonsterService(string apiKey, Uri baseUri, HttpClient? httpClient = null)
        : base(apiKey, baseUri, httpClient)
    {
        SupportedCaptchaTypes = CaptchaType.ImageCaptcha | CaptchaType.ReCaptchaV2 | CaptchaType.ReCaptchaV3;
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync
        (string base64, ImageCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (!base64.StartsWith("base64,"))
        {
            base64 = "base64," + base64;
        }   

        var response = await HttpClient.PostToStringAsync(
                "in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("method", "base64")
                    .Add("body", base64)
                    .Add(ConvertCapabilities(options)),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<StringResponse>(
            response, CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false);
    }
}
