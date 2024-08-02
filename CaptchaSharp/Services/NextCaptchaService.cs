using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.AntiCaptcha.Responses;
using CaptchaSharp.Models.NextCaptcha.Requests.Tasks;
using CaptchaSharp.Models.NextCaptcha.Requests.Tasks.Proxied;

namespace CaptchaSharp.Services;

/// <summary>
/// The service offered by https://nextcaptcha.com/
/// </summary>
public class NextCaptchaService : CustomAntiCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="NextCaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public NextCaptchaService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("https://api.nextcaptcha.com"), httpClient)
    {
        SupportedCaptchaTypes =
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3 |
            CaptchaType.FunCaptcha |
            CaptchaType.HCaptcha;

        SoftId = null;
    }

    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaMobileAsync(
        string appPackageName, string appKey, string appAction, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();

        if (proxy is not null)
        {
            content.Task = new RecaptchaMobileTask
            {
                AppPackageName = appPackageName,
                AppKey = appKey,
                AppAction = appAction,
            }.SetProxy(proxy);
        }
        else
        {
            content.Task = new RecaptchaMobileTaskProxyless
            {
                AppPackageName = appPackageName,
                AppKey = appKey,
                AppAction = appAction,
            };
        }
        
        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(response, CaptchaType.ReCaptchaMobile,
            cancellationToken).ConfigureAwait(false);
    }
    #endregion
}
