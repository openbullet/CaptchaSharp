using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp
{
    /// <summary>Abstract class for a generic captcha solving service.</summary>
    public abstract class CaptchaService
    {
        /// <summary>The maximum allowed time for captcha completion.
        /// If this <see cref="TimeSpan"/> is exceeded, a <see cref="TimeoutException"/> is thrown.</summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);

        /// <summary>Returns a list of flags that denote the capabilities of the service in terms of additional 
        /// parameters to provide when solving text or image based captchas.</summary>
        public virtual CaptchaServiceCapabilities Capabilities => CaptchaServiceCapabilities.None;

        /// <summary>Retrieves the remaining balance in USD as a <see cref="double"/>.</summary>
        /// <exception cref="BadAuthenticationException">Thrown when the provided credentials are invalid.</exception>
        public virtual Task<decimal> GetBalanceAsync
            (CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <summary>Solves a text based captcha.</summary>
        /// 
        /// <param name="text">The captcha question.</param>
        /// 
        /// <param name="options">
        /// Any additional options like the language of the question.
        /// If null they will be disregarded.
        /// </param>
        /// 
        /// <param name="cancellationToken">A token that can be used to cancel the async task.</param>
        /// 
        /// <returns>
        /// A <see cref="StringResponse"/> containing the captcha id to be used with 
        /// <see cref="ReportSolution(long, CaptchaType, bool, CancellationToken)"/> and the 
        /// captcha solution as plaintext.
        /// </returns>
        public virtual Task<StringResponse> SolveTextCaptchaAsync
            (string text, TextCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <summary>Solves an image based captcha.</summary>
        /// 
        /// <param name="base64">The captcha image encoded as a base64 string.</param>
        /// 
        /// <param name="options">
        /// Any additional options like whether the captcha is case sensitive.
        /// If null they will be disregarded.
        /// </param>
        /// 
        /// <param name="cancellationToken">A token that can be used to cancel the async task.</param>
        /// 
        /// /// <returns>
        /// A <see cref="StringResponse"/> containing the captcha id to be used with 
        /// <see cref="ReportSolution(long, CaptchaType, bool, CancellationToken)"/> and the 
        /// captcha solution as plaintext.
        /// </returns>
        public virtual Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <summary>Solves a Google ReCaptcha V2.</summary>
        /// 
        /// <param name="siteKey">The site key, can be found in the webpage source or by sniffing requests.</param>
        /// <param name="siteUrl">The URL where the captcha appears.</param>
        /// <param name="invisible">Whether the captcha is not in a clickable format on the page.</param>
        /// 
        /// <param name="proxy">
        /// A proxy that can be used by the captcha service to fetch the captcha challenge from the same IP you are 
        /// going to send it from when you submit the form. It can help bypass some blocks. If null, the service will 
        /// fetch the captcha without using a proxy.
        /// </param>
        /// 
        /// <param name="cancellationToken">A token that can be used to cancel the async task.</param>
        /// 
        /// <returns>
        /// A <see cref="StringResponse"/> containing the captcha id to be used with 
        /// <see cref="ReportSolution(long, CaptchaType, bool, CancellationToken)"/> and the 
        /// captcha solution as plaintext.
        /// </returns>
        public virtual Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <summary>Solves a Google ReCaptcha V3.</summary>
        /// 
        /// <param name="siteKey">The site key, can be found in the webpage source or by sniffing requests.</param>
        /// <param name="siteUrl">The URL where the captcha appears.</param>
        /// <param name="action">The action to execute. Can be found in the webpage source or in a js file.</param>
        /// <param name="minScore">The minimum human-to-robot score necessary to solve the challenge.</param>
        /// 
        /// <param name="proxy">
        /// A proxy that can be used by the captcha service to fetch the captcha challenge from the same IP you are 
        /// going to send it from when you submit the form. It can help bypass some blocks. If null, the service will 
        /// fetch the captcha without using a proxy.
        /// </param>
        /// 
        /// <param name="cancellationToken">A token that can be used to cancel the async task.</param>
        /// 
        /// <returns>
        /// A <see cref="StringResponse"/> containing the captcha id to be used with 
        /// <see cref="ReportSolution(long, CaptchaType, bool, CancellationToken)"/> and the 
        /// captcha solution as plaintext.
        /// </returns>
        public virtual Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action, float minScore, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <summary>Solves a Google ReCaptcha V3.</summary>
        /// 
        /// <param name="publicKey">
        /// Can be found inside data-pkey parameter of funcaptcha's div element or inside an input element 
        /// with name <code>fc-token</code>. Just extract the key indicated after pk from the value of this element.
        /// </param>
        /// 
        /// <param name="serviceUrl">
        /// Can be found in the <code>fc-token</code>, that is a value of the <code>surl</code> parameter.
        /// </param>
        /// 
        /// <param name="siteUrl">The URL where the captcha appears.</param>
        /// 
        /// <param name="noJS">
        /// Whether to solve the challenge without JavaScript enabled. This is not supported by every service and 
        /// it provides a partial token.
        /// </param>
        /// 
        /// <param name="proxy">
        /// A proxy that can be used by the captcha service to fetch the captcha challenge from the same IP you are 
        /// going to send it from when you submit the form. It can help bypass some blocks. If null, the service will 
        /// fetch the captcha without using a proxy.
        /// </param>
        /// 
        /// <param name="cancellationToken">A token that can be used to cancel the async task.</param>
        /// 
        /// <returns>
        /// A <see cref="StringResponse"/> containing the captcha id to be used with 
        /// <see cref="ReportSolution(long, CaptchaType, bool, CancellationToken)"/> and the 
        /// captcha solution as plaintext.
        /// </returns>
        public virtual Task<StringResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <summary>Solves a HCaptcha.</summary>
        /// 
        /// <param name="siteKey">The site key, can be found in the webpage source or by sniffing requests.</param>
        /// <param name="siteUrl">The URL where the captcha appears.</param>
        /// 
        /// <param name="proxy">
        /// A proxy that can be used by the captcha service to fetch the captcha challenge from the same IP you are 
        /// going to send it from when you submit the form. It can help bypass some blocks. If null, the service will 
        /// fetch the captcha without using a proxy.
        /// </param>
        /// 
        /// <param name="cancellationToken">A token that can be used to cancel the async task.</param>
        /// 
        /// <returns>
        /// A <see cref="StringResponse"/> containing the captcha id to be used with 
        /// <see cref="ReportSolution(long, CaptchaType, bool, CancellationToken)"/> and the 
        /// captcha solution as plaintext.
        /// </returns>
        public virtual Task<StringResponse> SolveHCaptchaAsync
            (string siteKey, string siteUrl, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<StringResponse> SolveKeyCaptchaAsync
            (string userId, string sessionId, string webServerSign1, string webServerSign2, string siteUrl,
            Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<GeeTestResponse> SolveGeeTestAsync
            (string gt, string challenge, string apiServer, string siteUrl, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<StringResponse> SolveCapyAsync
            (string siteKey, string siteUrl, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task ReportSolution
            (long id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        internal async Task<CaptchaResponse> TryGetResult
            (CaptchaTask task, CancellationToken cancellationToken = default)
        {
            var start = DateTime.Now;
            CaptchaResponse result;

            // Initial 5s delay
            await Delay(5000);

            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                result = await CheckResult(task, cancellationToken);
                await Delay(5000);
            }
            while (!task.Completed && DateTime.Now - start < Timeout);

            if (!task.Completed)
                throw new TimeoutException();

            return result;
        }

        internal virtual Task<CaptchaResponse> CheckResult
            (CaptchaTask task, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private async Task Delay(int milliseconds)
        {
            await Task.Run(() => Thread.Sleep(milliseconds));
        }
    }
}
