using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp;

/// <summary>Abstract class for a generic captcha solving service.</summary>
public abstract class CaptchaService
{
    /// <summary>The maximum allowed time for captcha completion.
    /// If this <see cref="TimeSpan"/> is exceeded, a <see cref="TimeoutException"/> is thrown.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(3);

    /// <summary>The interval at which the service will be polled for a solution.</summary>
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>Returns a list of flags that denote the capabilities of the service in terms of additional 
    /// parameters to provide when solving text or image based captchas.</summary>
    public virtual CaptchaServiceCapabilities Capabilities => CaptchaServiceCapabilities.None;

    /// <summary>Retrieves the remaining balance in USD as a <see cref="double"/>.</summary>
    /// <exception cref="BadAuthenticationException">Thrown when the provided credentials are invalid.</exception>
    public virtual Task<decimal> GetBalanceAsync(
        CancellationToken cancellationToken = default)
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
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<StringResponse> SolveTextCaptchaAsync(
        string text, TextCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>Solves an image based captcha.</summary>
    /// 
    /// <param name="base64">The captcha image encoded as a base64 string.</param>
    /// 
    /// <param name="options">
    /// Any additional options like whether the captcha is case-sensitive.
    /// If null they will be disregarded.
    /// </param>
    /// 
    /// <param name="cancellationToken">A token that can be used to cancel the async task.</param>
    /// 
    /// /// <returns>
    /// A <see cref="StringResponse"/> containing the captcha id to be used with 
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>Solves a Google ReCaptcha V2.</summary>
    /// 
    /// <param name="siteKey">The site key, can be found in the webpage source or by sniffing requests.</param>
    /// <param name="siteUrl">The URL where the captcha appears.</param>
    /// <param name="dataS">The value of the 's' or 'data-s' field (currently only for Google services).</param>
    /// <param name="enterprise">Whether this is an Enterprise ReCaptcha V2.</param>
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
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
    
    /// <summary>Solves a Google ReCaptcha V3.</summary>
    /// 
    /// <param name="siteKey">The site key, can be found in the webpage source or by sniffing requests.</param>
    /// <param name="siteUrl">The URL where the captcha appears.</param>
    /// <param name="action">The action to execute. Can be found in the webpage source or in a js file.</param>
    /// <param name="minScore">The minimum human-to-robot score necessary to solve the challenge.</param>
    /// <param name="enterprise">Whether this is an Enterprise ReCaptcha V3.</param>
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
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>Solves a FunCaptcha (Arkose Labs).</summary>
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
    /// <param name="noJs">
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
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl,
        bool noJs = false, Proxy? proxy = null,
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
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>Solves a KeyCaptcha.</summary>
    /// 
    /// <param name="userId">s_s_c_user_id parameter in the webpage source code.</param>
    /// <param name="sessionId">s_s_c_session_id parameter in the webpage source code.</param>
    /// <param name="webServerSign1">s_s_c_web_server_sign parameter in the webpage source code.</param>
    /// <param name="webServerSign2">s_s_c_web_server_sign2 parameter in the webpage source code.</param>
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
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<StringResponse> SolveKeyCaptchaAsync(
        string userId, string sessionId, string webServerSign1, string webServerSign2, string siteUrl,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>Solves a GeeTest captcha.</summary>
    /// 
    /// <param name="gt">The static public key assigned to the website found in the webpage source code.</param>
    /// <param name="challenge">The dynamic challenge key found in the webpage source code.</param>
    /// <param name="apiServer">The api_server parameter found in the webpage source code.</param>
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
    /// A <see cref="GeeTestResponse"/> containing the captcha id to be used with 
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and three solution parameters 
    /// (Challenge, Validate and SecCode) that you will need to provide when you submit the form.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>Solves a Capy captcha.</summary>
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
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<CapyResponse> SolveCapyAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>Solves a DataDome captcha.</summary>
    /// 
    /// <param name="siteUrl">The URL where the captcha appears.</param>
    /// <param name="captchaUrl">The URL of the captcha. It is obtained from the 'dd' object in a script
    /// inside the HTML and the 'datadome' cookie</param>
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
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext (a.k.a. a valid datadome session cookie).
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<StringResponse> SolveDataDomeAsync(
        string siteUrl, string captchaUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
        
    /// <summary>Solves a Cloudflare Turnstile captcha.</summary>
    /// 
    /// <param name="siteKey">The site key, can be found in the webpage source or by sniffing requests.</param>
    /// <param name="siteUrl">The URL where the captcha appears.</param>
    /// <param name="action">Value of optional action parameter you found on page, can be defined in data-action attribute or passed to turnstile.render call.</param>
    /// <param name="data">The value of cData passed to turnstile.render call. Also can be defined in data-cdata attribute.</param>
    /// <param name="pageData">The value of chlPageData passed to turnstile.render call.</param>
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
    /// <see cref="ReportSolution(string, CaptchaType, bool, CancellationToken)"/> and the 
    /// captcha solution as plaintext.
    /// </returns>
    /// 
    /// <exception cref="TaskCreationException"></exception>
    /// <exception cref="TaskSolutionException"></exception>
    /// <exception cref="TimeoutException"></exception>
    public virtual Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
    
    /// <summary>
    /// Reports a captcha solution as good or bad to the service.
    /// Mostly used for reporting bad solutions for image captchas and get the funds back.
    /// Make sure to not abuse this system or the service might ban your account!
    /// </summary>
    /// 
    /// <param name="id">The string ID of the captcha that you got inside your <see cref="CaptchaResponse"/>.</param>
    /// <param name="type">The type of captcha you want to report.</param>
    /// 
    /// <param name="correct">
    /// If true, the captcha will be reported as correctly solved (this is not supported by some services).
    /// </param>
    /// 
    /// <param name="cancellationToken">A token that can be used to cancel the async task.</param>
    /// 
    /// <exception cref="TaskReportException"></exception>
    public virtual Task ReportSolution(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary></summary>
    protected async Task<T> GetResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        var start = DateTime.Now;
        T? result;

        // Initial delay
        await Task.Delay(PollingInterval, cancellationToken);

        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            result = await CheckResult<T>(task, cancellationToken);
            await Task.Delay(PollingInterval, cancellationToken);
        }
        while (!task.Completed && DateTime.Now - start < Timeout);

        if (!task.Completed || result is null)
        {
            throw new TimeoutException();
        }

        return result;
    }

    /// <summary></summary>
    protected virtual Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        throw new NotImplementedException();
    }
}
