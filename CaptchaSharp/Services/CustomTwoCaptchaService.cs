using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by a service that implements the 2captcha API.
/// </summary>
public class CustomTwoCaptchaService : TwoCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="CustomTwoCaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="baseUri">The base URI of the service.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    /// <param name="overrideHostHeader">Whether to override the Host header to 2captcha.com.</param>
    public CustomTwoCaptchaService(
        string apiKey, Uri baseUri, HttpClient? httpClient = null, bool overrideHostHeader = true)
        : base(apiKey, httpClient)
    {
        SetupHttpClient(baseUri, overrideHostHeader);

        // Services that implement the 2captcha API don't always support
        // JSON responses, so we will not set the json=1 flag
        UseJsonFlag = false;
    }

    private void SetupHttpClient(Uri baseUri, bool overrideHostHeader = true)
    {
        if (overrideHostHeader)
        {
            // Use 2captcha.com as host header to simulate an entry in the hosts file
            HttpClient.DefaultRequestHeaders.Host = "2captcha.com";
        }
            
        HttpClient.BaseAddress = baseUri;
    }

    #region Supported Types
    /// <summary>The supported captcha types for this service.</summary>
    public CaptchaType SupportedCaptchaTypes { get; set; } =
        CaptchaType.TextCaptcha |
        CaptchaType.ImageCaptcha |
        CaptchaType.ReCaptchaV2 |
        CaptchaType.ReCaptchaV3 |
        CaptchaType.FunCaptcha |
        CaptchaType.HCaptcha |
        CaptchaType.KeyCaptcha |
        CaptchaType.GeeTest |
        CaptchaType.Capy |
        CaptchaType.DataDome |
        CaptchaType.CloudflareTurnstile;
    #endregion
}
