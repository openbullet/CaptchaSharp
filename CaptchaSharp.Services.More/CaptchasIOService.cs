using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More;

/// <summary>
/// The service provided by <c>https://captchas.io/</c>
/// </summary>
public class CaptchasIOService : CustomTwoCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="CaptchasIOService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public CaptchasIOService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("https://api.captchas.io"), httpClient, false) 
    {
        SupportedCaptchaTypes =
            CaptchaType.ImageCaptcha |
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3;
    }
}
