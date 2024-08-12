using CaptchaSharp.Enums;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.CaptchaOptions;
using CaptchaSharp.Models.CaptchaResponses;
using CaptchaSharp.Models.TwoCaptcha;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://captchas.io/
/// </summary>
public class CaptchasIoService : CustomTwoCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="CaptchasIoService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public CaptchasIoService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("https://api.captchas.io"), httpClient, false)
    {
        SupportedCaptchaTypes =
            CaptchaType.TextCaptcha |
            CaptchaType.ImageCaptcha |
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3 |
            CaptchaType.FunCaptcha |
            CaptchaType.HCaptcha |
            CaptchaType.GeeTest |
            CaptchaType.CloudflareTurnstile;
    }
    
    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveAudioCaptchaAsync(
        string base64, AudioCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var multipartData = new StringPairCollection()
            .Add("key", ApiKey)
            .Add("method", "audio")
            .Add("json", UseJsonFlag ? "1" : null)
            .Add("header_acao", AddAcaoHeader ? "1" : null)
            .ToMultipartFormDataContent();
        
        // Add the "file" as a multipart file from base64 (convert to byte array)
        multipartData.Add(new ByteArrayContent(Convert.FromBase64String(base64), 0, 0), "file", "audio.mp3");
        
        var response = await HttpClient.PostMultipartToStringAsync("in.php",
                multipartData,
                cancellationToken)
            .ConfigureAwait(false);
        
        var captchaResponse = UseJsonFlag
            ? await GetResultAsync<StringResponse>(
                response.Deserialize<TwoCaptchaResponse>(), CaptchaType.AudioCaptcha,
                cancellationToken).ConfigureAwait(false)
            : await GetResultAsync<StringResponse>(
                response, CaptchaType.AudioCaptcha,
                cancellationToken).ConfigureAwait(false);
        
        if (captchaResponse.Response.StartsWith("error_", StringComparison.InvariantCultureIgnoreCase))
        {
            throw new TaskSolutionException(captchaResponse.Response);
        }
        
        return captchaResponse;
    }
    #endregion
    
    #region Getting the result
    private async Task<T> GetResultAsync<T>(
        TwoCaptchaResponse twoCaptchaResponse, CaptchaType type, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (twoCaptchaResponse.IsErrorCode)
        {
            throw new TaskCreationException(twoCaptchaResponse.Request!);
        }

        var task = new CaptchaTask(twoCaptchaResponse.Request!, type);

        return await GetResultAsync<T>(task, cancellationToken).ConfigureAwait(false);
    }
    #endregion
}
