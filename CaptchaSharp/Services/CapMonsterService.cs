using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services
{
    /// <summary>The service provided by the CapMonster OCR application by ZennoLab.</summary>
    public class CapMonsterService : CustomTwoCaptchaService
    {
        /// <summary>Initializes a <see cref="CapMonsterService"/> using the given <paramref name="apiKey"/>, 
        /// <paramref name="baseUri"/> and <paramref name="httpClient"/>.
        /// If <paramref name="httpClient"/> is null, a default one will be created.</summary>
        public CapMonsterService(string apiKey, Uri baseUri, HttpClient httpClient = null)
            : base(apiKey, baseUri, httpClient)
        {
            SupportedCaptchaTypes = CaptchaType.ImageCaptcha | CaptchaType.ReCaptchaV2 | CaptchaType.ReCaptchaV3;
        }

        /// <inheritdoc/>
        public async override Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            if (!base64.StartsWith("base64,"))
            {
                base64 = "base64," + base64;
            }   

            var response = await httpClient.PostToStringAsync
                ("in.php",
                new StringPairCollection()
                    .Add("key", ApiKey)
                    .Add("method", "base64")
                    .Add("body", base64)
                    .Add(ConvertCapabilities(options)),
                cancellationToken)
                .ConfigureAwait(false);

            return (await TryGetResult(response, CaptchaType.ImageCaptcha, cancellationToken).ConfigureAwait(false)) as StringResponse;
        }
    }
}
