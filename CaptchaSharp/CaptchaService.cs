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

        public CaptchaServiceCapabilities Capabilities { get; } = CaptchaServiceCapabilities.None;

        public virtual Task<decimal> GetBalanceAsync
            (CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<CaptchaResponse> SolveTextCaptchaAsync
            (string text, TextCaptchaOptions options = default, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<CaptchaResponse> SolveImageCaptchaAsync
            (Bitmap image, ImageCaptchaOptions options = default, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<CaptchaResponse> SolveRecaptchaV2Async
            (string siteKey, string siteUrl, bool invisible = false, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public virtual Task<CaptchaResponse> SolveRecaptchaV3Async
            (string siteKey, string siteUrl, string action, float minScore = 0.3F, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
        
        public virtual Task<CaptchaResponse> SolveFuncaptchaAsync
            ()
        {
            throw new NotSupportedException();
        }

        public virtual Task<CaptchaResponse> SolveHCaptchaAsync
            ()
        {
            throw new NotSupportedException();
        }

        public virtual Task ReportIncorrectSolution(string taskId)
        {
            throw new NotSupportedException();
        }

        protected async Task<CaptchaResponse> TryGetResult
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

        protected virtual Task<CaptchaResponse> CheckResult
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
