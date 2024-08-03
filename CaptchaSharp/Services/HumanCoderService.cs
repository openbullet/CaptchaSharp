using System;
using System.Net.Http;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://humancoder.com/
/// </summary>
public class HumanCoderService : CaptchaCoderService
{
    /// <summary>
    /// Initializes a <see cref="HumanCoderService"/>.
    /// </summary>
    public HumanCoderService(string apiKey, HttpClient? httpClient = null) : base(apiKey, httpClient)
    {
        this.HttpClient.BaseAddress = new Uri("http://fasttypers.org");
    }
}
