using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://captchaai.com/
/// </summary>
public class CaptchaAiService : CustomTwoCaptchaService
{
    /// <summary>
    /// Initializes a <see cref="CaptchaAiService"/>.
    /// </summary>
    /// <param name="apiKey">The API key to use.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public CaptchaAiService(string apiKey, HttpClient? httpClient = null)
        : base(apiKey, new Uri("http://ocr.captchaai.com"), httpClient) { }
    
    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolutionAsync(
        string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var action = correct ? "reportgood" : "reportbad";

        // This service just replies with 200 OK without body
        // when reporting a solution, so we don't need to check the response
        using var response = await HttpClient.GetAsync("res.php",
            new StringPairCollection()
                .Add("key", ApiKey)
                .Add("action", action)
                .Add("id", id)
                .Add("json", Convert.ToInt32(UseJsonFlag).ToString()),
            cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            throw new TaskReportException("Failed to report the solution.");
        }
    }
    #endregion
}
