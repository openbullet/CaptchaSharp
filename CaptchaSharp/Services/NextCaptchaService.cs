using System;
using System.Net.Http;
using CaptchaSharp.Enums;

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
}
