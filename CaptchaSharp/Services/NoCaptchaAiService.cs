using System;
using System.Net.Http;
using CaptchaSharp.Enums;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by <c>https://nocaptchaai.com/</c>
/// </summary>
public class NoCaptchaAiService : CustomTwoCaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The default <see cref="HttpClient"/> used for requests.
    /// </summary>
    private readonly HttpClient _httpClient;
    
    /// <summary>
    /// Initializes a <see cref="NoCaptchaAiService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public NoCaptchaAiService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("https://token.nocaptchaai.com/"), httpClient, false)
    {
        ApiKey = apiKey;
        _httpClient = httpClient ?? new HttpClient();
        
        _httpClient.BaseAddress = new Uri("https://free.nocaptchaai.com");

        SupportedCaptchaTypes =
            CaptchaType.ImageCaptcha |
            CaptchaType.HCaptcha;
    }
}
