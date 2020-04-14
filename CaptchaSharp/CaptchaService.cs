using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp
{
    public abstract class CaptchaService
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);

        public virtual CaptchaServiceCapabilities Capabilities => CaptchaServiceCapabilities.None;

        public virtual Task<decimal> GetBalanceAsync
            (CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<StringResponse> SolveTextCaptchaAsync
            (string text, TextCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<StringResponse> SolveImageCaptchaAsync
            (string base64, ImageCaptchaOptions options = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<StringResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<StringResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action, float minScore, Proxy proxy = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
        
        public virtual Task<StringResponse> SolveFuncaptchaAsync
            (string publicKey, string serviceUrl, string siteUrl, bool noJS = false, Proxy proxy = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

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
