using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More;

/// <summary>
/// The service provided by <c>https://rucaptcha.com/</c>
/// </summary>
public class RuCaptchaService : CustomTwoCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="RuCaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public RuCaptchaService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("http://rucaptcha.com"), httpClient) { }
}
