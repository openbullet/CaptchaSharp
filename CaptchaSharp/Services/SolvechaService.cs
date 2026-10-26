using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.CaptchaOptions;
using CaptchaSharp.Models.CaptchaResponses;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://solvecha.com/
/// Only image (text) captchas are supported.
/// </summary>
public class SolvechaService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Initializes a <see cref="SolvechaService"/>.
    /// </summary>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public SolvechaService(string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        ApiKey = apiKey;
        HttpClient.BaseAddress = new Uri("https://api.solvecha.com");
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostJsonToStringAsync(
                "getBalance",
                new { ClientKey = ApiKey },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var jObject = JObject.Parse(response);

        if (IsError(jObject))
        {
            throw new BadAuthenticationException(GetErrorMessage(jObject));
        }

        return jObject["balance"]!.Value<decimal>();
    }
    #endregion

    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(base64))
        {
            throw new ArgumentException("The image base64 string is null or empty", nameof(base64));
        }

        var response = await HttpClient.PostJsonToStringAsync(
                "createTask",
                new
                {
                    ClientKey = ApiKey,
                    Task = new { Type = "ImageToTextTask", Body = base64 }
                },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var jObject = JObject.Parse(response);

        if (IsError(jObject))
        {
            throw new TaskCreationException(GetErrorMessage(jObject));
        }

        var task = new CaptchaTask(GetTaskId(jObject), CaptchaType.ImageCaptcha);

        // The API waits for the answer for a few seconds and returns it right away if it is ready
        if (jObject["status"]?.Value<string>() == "ready")
        {
            task.Completed = true;
            return ToStringResponse(jObject, task);
        }

        return await GetResultAsync<StringResponse>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResultAsync<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await HttpClient.PostJsonToStringAsync(
                "getTaskResult",
                new { ClientKey = ApiKey, TaskId = task.Id },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var jObject = JObject.Parse(response);

        if (IsError(jObject))
        {
            task.Completed = true;
            throw new TaskSolutionException(GetErrorMessage(jObject));
        }

        if (jObject["status"]?.Value<string>() != "ready")
        {
            return null;
        }

        task.Completed = true;
        return ToStringResponse(jObject, task) as T;
    }
    #endregion

    #region Private Methods
    private static bool IsError(JObject jObject)
        => jObject["errorId"]?.Value<int>() is not null and not 0;

    private static string GetErrorMessage(JObject jObject)
        => $"{jObject["errorCode"]}: {jObject["errorDescription"]}";

    private static string GetTaskId(JObject jObject)
        => jObject["taskId"]?.ToString() ?? throw new TaskCreationException("The API returned no task id");

    private static StringResponse ToStringResponse(JObject jObject, CaptchaTask task)
    {
        var text = jObject["solution"]?["text"]?.Value<string>()
            ?? throw new TaskSolutionException("The solution is null");

        return new StringResponse { Id = task.Id, Response = text };
    }
    #endregion
}
