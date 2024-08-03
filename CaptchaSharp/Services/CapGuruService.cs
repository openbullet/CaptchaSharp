using System;
using System.Net.Http;
using CaptchaSharp.Enums;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://cap.guru/
/// </summary>
public class CapGuruService : CustomTwoCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="CapGuruService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public CapGuruService(string apiKey, HttpClient? httpClient = null) 
        : base(apiKey, new Uri("http://api.cap.guru"), httpClient)
    {
        SupportedCaptchaTypes =
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3 |
            CaptchaType.HCaptcha |
            CaptchaType.CloudflareTurnstile;
    }
}
