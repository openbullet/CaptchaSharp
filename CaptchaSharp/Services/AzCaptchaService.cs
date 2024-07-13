using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by <c>https://azcaptcha.com/</c>
/// </summary>
public class AzCaptchaService : CustomTwoCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="AzCaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public AzCaptchaService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("http://azcaptcha.com"), httpClient, false)
    {
        SupportedCaptchaTypes =
            CaptchaType.ImageCaptcha |
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3;
    }
}
