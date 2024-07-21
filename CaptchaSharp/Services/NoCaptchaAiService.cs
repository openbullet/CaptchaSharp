using System;
using System.Net.Http;
using CaptchaSharp.Enums;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://nocaptchaai.com/
/// </summary>
public class NoCaptchaAiService : CustomTwoCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="NoCaptchaAiService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public NoCaptchaAiService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("https://token.nocaptchaai.com/"), httpClient, false)
    {
        ApiKey = apiKey;
        HttpClient.BaseAddress = new Uri("https://free.nocaptchaai.com");

        SupportedCaptchaTypes =
            CaptchaType.ImageCaptcha |
            CaptchaType.HCaptcha;
    }
}
