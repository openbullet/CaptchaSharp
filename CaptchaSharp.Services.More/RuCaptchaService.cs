using System;
using System.Net.Http;

namespace CaptchaSharp.Services.More
{
    /// <summary>The service provided by <c>https://rucaptcha.com/</c></summary>
    public class RuCaptchaService : CustomTwoCaptchaService
    {
        /// <summary>Initializes a <see cref="RuCaptchaService"/> using the given <paramref name="apiKey"/> and 
        /// <paramref name="httpClient"/>. If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public RuCaptchaService(string apiKey, HttpClient? httpClient = null)
            : base(apiKey, new Uri("http://rucaptcha.com"), httpClient) { }
    }
}
