using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models.CaptchaOptions;
using CaptchaSharp.Models.CaptchaResponses;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://captchacoder.com/
/// </summary>
public class CaptchaCoderService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Initializes a <see cref="CaptchaCoderService"/>.
    /// </summary>
    public CaptchaCoderService(string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        ApiKey = apiKey;
        this.HttpClient.BaseAddress = new Uri("http://api.captchacoder.com/");
            
        // Since this service replies directly with the solution to the task creation request
        // we need to set a high timeout here, or it will not finish in time
        this.HttpClient.Timeout = Timeout;
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync(
            "Imagepost.ashx",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", "balance")
                .ToMultipartFormDataContent(), cancellationToken)
            .ConfigureAwait(false);

        if (decimal.TryParse(response, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal balance))
        {
            return balance;
        }

        throw new BadAuthenticationException(response);
    }
    #endregion

    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null, CancellationToken cancellationToken = default)
    {
        var captchaId = Guid.NewGuid().ToString();
        
        var response = await HttpClient.PostMultipartToStringAsync(
            "Imagepost.ashx",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", "upload")
                .Add("file", base64)
                .Add("gen_task_id", captchaId)
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);
        
        if (response.Contains("Error"))
        {
            throw new TaskSolutionException(response);
        }

        return new StringResponse { Id = captchaId, Response = response };
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        if (proxy is not null)
        {
            throw new NotSupportedException("Proxies are not supported by this service.");
        }
        
        var captchaId = Guid.NewGuid().ToString();

        var response = await HttpClient.PostMultipartToStringAsync(
            "Imagepost.ashx",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", "upload")
                .Add("captchatype", 3)
                .Add("gen_task_id", captchaId)
                .Add("pageurl", siteUrl)
                .Add("sitekey", siteKey)
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        if (response.Contains("Error"))
        {
            throw new TaskSolutionException(response);
        }
        
        return new StringResponse { Id = captchaId, Response = response };
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        if (proxy is not null)
        {
            throw new NotSupportedException("Proxies are not supported by this service.");
        }
        
        var captchaId = Guid.NewGuid().ToString();
        
        var response = await HttpClient.PostMultipartToStringAsync(
            "Imagepost.ashx",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", "upload")
                .Add("captchatype", 3)
                .Add("gen_task_id", captchaId)
                .Add("pageurl", siteUrl)
                .Add("sitekey", siteKey)
                .Add("pageaction", action)
                .ToMultipartFormDataContent(),
            cancellationToken)
            .ConfigureAwait(false);

        if (response.Contains("Error"))
        {
            throw new TaskSolutionException(response);
        }
        
        return new StringResponse { Id = captchaId, Response = response };
    }

    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolutionAsync(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostMultipartToStringAsync(
            "Imagepost.ashx",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", "refund")
                .Add("gen_task_id", id)
                .ToMultipartFormDataContent(), cancellationToken)
            .ConfigureAwait(false);

        if (!response.Contains("ok", StringComparison.CurrentCultureIgnoreCase))
        {
            throw new TaskReportException(response);
        }
    }
    #endregion
}
