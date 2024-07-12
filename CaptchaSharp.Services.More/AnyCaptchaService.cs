using CaptchaSharp.Enums;
using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More;

/// <summary>
/// The service provided by <c>https://anycaptcha.com/</c>
/// </summary>
public class AnyCaptchaService : CustomAntiCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="AnyCaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public AnyCaptchaService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("https://api.anycaptcha.com"), httpClient)
    {
        SupportedCaptchaTypes =
            CaptchaType.ImageCaptcha |
            CaptchaType.ReCaptchaV2 |
            CaptchaType.ReCaptchaV3 |
            CaptchaType.FunCaptcha |
            CaptchaType.HCaptcha;
    }
}
