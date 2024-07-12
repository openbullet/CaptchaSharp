using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by a service that implements the anti-captcha API.
/// </summary>
public class CustomAntiCaptchaService : AntiCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="CustomAntiCaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="baseUri">The base URI of the service.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public CustomAntiCaptchaService(string apiKey, Uri baseUri, HttpClient? httpClient = null)
        : base(apiKey, httpClient)
    {
        httpClient.BaseAddress = baseUri;
    }

    #region Supported Types
    /// <summary>The supported captcha types for this service.</summary>
    public CaptchaType SupportedCaptchaTypes { get; set; } =
        CaptchaType.ImageCaptcha |
        CaptchaType.ReCaptchaV2 |
        CaptchaType.ReCaptchaV3 |
        CaptchaType.FunCaptcha |
        CaptchaType.HCaptcha |
        CaptchaType.GeeTest;
    #endregion
}
