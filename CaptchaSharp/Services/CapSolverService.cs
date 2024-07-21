using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.CapSolver.Requests;
using CaptchaSharp.Models.CapSolver.Requests.Tasks;
using CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;
using CaptchaSharp.Models.CapSolver.Responses;
using CaptchaSharp.Models.CapSolver.Responses.Solutions;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by <c>https://capsolver.com/</c>
/// </summary>
public class CapSolverService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The default <see cref="HttpClient"/> used for requests.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// The ID of the app.
    /// </summary>
    private const string _appId = "FE552FC0-8A06-4B44-BD30-5B9DDA2A4194";

    /// <summary>
    /// Initializes a <see cref="CapSolverService"/>.
    /// </summary>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public CapSolverService(string apiKey, HttpClient? httpClient = null)
    {
        ApiKey = apiKey;
        this._httpClient = httpClient ?? new HttpClient();
        this._httpClient.BaseAddress = new Uri("https://api.capsolver.com");
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostJsonAsync<GetBalanceResponse>(
                "getBalance",
                new Request { ClientKey = ApiKey },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (response.IsError)
        {
            throw new BadAuthenticationException($"{response.ErrorCode}: {response.ErrorDescription}");
        }

        return new decimal(response.Balance);
    }
    #endregion

    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostJsonToStringAsync(
                "createTask",
                new CaptchaTaskRequest
                {
                    ClientKey = ApiKey,
                    AppId = _appId,
                    Task = new ImageCaptchaTask
                    {
                        Body = base64
                    }
                },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // The image captcha task immediately returns the solution
        var result = response.Deserialize<GetTaskResultResponse>();

        if (result.IsError)
        {
            throw new TaskSolutionException($"{result.ErrorCode}: {result.ErrorDescription}");
        }

        var jObject = JObject.Parse(response);
        var solution = jObject["solution"];
        var taskId = jObject["taskId"]!.Value<string>()!;

        result.Solution = solution!.ToObject<ImageCaptchaSolution>()!;

        return (result.Solution.ToCaptchaResponse(taskId) as StringResponse)!;
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
        
        // If dataS is not null or empty, the enterprise payload is { "s": dataS }
        var enterprisePayload = string.IsNullOrEmpty(dataS)
            ? null
            : JObject.Parse($"{{ \"s\": \"{dataS}\" }}");
        
        if (enterprise)
        {
            if (proxy is not null)
            {
                content.Task = new RecaptchaV2EnterpriseTask
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    EnterprisePayload = enterprisePayload
                }.SetProxy(proxy);
            }
            else
            {
                content.Task = new RecaptchaV2EnterpriseTaskProxyless
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    EnterprisePayload = enterprisePayload
                };
            }
        }
        else
        {
            if (proxy is not null)
            {
                content.Task = new RecaptchaV2Task
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    IsInvisible = invisible,
                }.SetProxy(proxy);
            }
            else
            {
                content.Task = new RecaptchaV2TaskProxyless
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    IsInvisible = invisible,
                };
            }
        }

        var response = await _httpClient.PostJsonAsync<TaskCreationResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(response, CaptchaType.ReCaptchaV2,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4F, bool enterprise = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();

        if (proxy is null)
        {
            content.Task = new RecaptchaV3TaskProxyless
            {
                WebsiteKey = siteKey,
                WebsiteURL = siteUrl,
                PageAction = action,
                MinScore = minScore,
                IsEnterprise = enterprise
            };
        }
        else
        {
            content.Task = new RecaptchaV3Task
            {
                WebsiteKey = siteKey,
                WebsiteURL = siteUrl,
                PageAction = action,
                MinScore = minScore,
                IsEnterprise = enterprise
            }.SetProxy(proxy);
        }

        var response = await _httpClient.PostJsonAsync<TaskCreationResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(response, CaptchaType.ReCaptchaV3,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        if (noJs)
        {
            throw new NotSupportedException("This service does not support no js solving");
        }

        var content = CreateTaskRequest();

        if (proxy is not null)
        {
            content.Task = new FunCaptchaTask
            {
                WebsitePublicKey = publicKey,
                WebsiteURL = siteUrl,
                FuncaptchaApiJSSubdomain = serviceUrl
            }.SetProxy(proxy);
        }
        else
        {
            content.Task = new FunCaptchaTaskProxyless
            {
                WebsitePublicKey = publicKey,
                WebsiteURL = siteUrl,
                FuncaptchaApiJSSubdomain = serviceUrl
            };
        }

        var response = await _httpClient.PostJsonAsync<TaskCreationResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(response, CaptchaType.FunCaptcha,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();

        if (proxy is not null)
        {
            content.Task = new HCaptchaTask
            {
                WebsiteKey = siteKey,
                WebsiteURL = siteUrl,
            }.SetProxy(proxy);
        }
        else
        {
            content.Task = new HCaptchaTaskProxyless
            {
                WebsiteKey = siteKey,
                WebsiteURL = siteUrl,
            };
        }

        var response = await _httpClient.PostJsonAsync<TaskCreationResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(response, CaptchaType.HCaptcha, 
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();

        if (proxy is not null)
        {
            content.Task = new GeeTestTask
            {
                WebsiteURL = siteUrl,
                Gt = gt,
                Challenge = challenge,
                GeetestApiServerSubdomain = apiServer
            }.SetProxy(proxy);
        }
        else
        {
            content.Task = new GeeTestTaskProxyless
            {
                WebsiteURL = siteUrl,
                Gt = gt,
                Challenge = challenge,
                GeetestApiServerSubdomain = apiServer,
                Version = 3
            };
        }

        var response = await _httpClient.PostJsonAsync<TaskCreationResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<GeeTestResponse>(response, CaptchaType.GeeTest,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveDataDomeAsync(
        string siteUrl, string captchaUrl, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();

        if (proxy is null)
        {
            throw new NotSupportedException("A proxy is required to solve a DataDome captcha");
        }

        content.Task = new DataDomeTask
        {
            WebsiteURL = siteUrl,
            CaptchaURL = captchaUrl
        }.SetProxy(proxy);

        var response = await _httpClient.PostJsonAsync<TaskCreationResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(response, CaptchaType.DataDome, 
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();

        content.Task = new AntiTurnstileTaskProxyless
        {
            WebsiteKey = siteKey,
            WebsiteURL = siteUrl,
            Metadata = new TurnstileMetadata
            {
                Action = string.IsNullOrEmpty(action) ? null : action,
                CData = string.IsNullOrEmpty(data) ? null : data
            }
        };
        
        var response = await _httpClient.PostJsonAsync<TaskCreationResponse>(
            "createTask",
            content,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<CloudflareTurnstileResponse>(
            response, CaptchaType.CloudflareTurnstile,
            cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region Getting the result
    private async Task<T> GetResult<T>(
        TaskCreationResponse response, CaptchaType type,
        CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (response.IsError)
        {
            throw new TaskCreationException($"{response.ErrorCode}: {response.ErrorDescription}");
        }

        var task = new CaptchaTask(response.TaskId, type);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await _httpClient.PostJsonToStringAsync(
            "getTaskResult",
            new GetTaskResultRequest { ClientKey = ApiKey, TaskId = task.Id },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var result = response.Deserialize<GetTaskResultResponse>();

        if (!result.IsReady)
        {
            return null;
        }

        task.Completed = true;

        if (result.IsError)
        {
            throw new TaskSolutionException($"{result.ErrorCode}: {result.ErrorDescription}");
        }

        var jObject = JObject.Parse(response);
        var solution = jObject["solution"];
            
        if (solution is null)
        {
            throw new TaskSolutionException("The solution is null");
        }

        result.Solution = task.Type switch
        {
            CaptchaType.ReCaptchaV2 or CaptchaType.ReCaptchaV3 or CaptchaType.HCaptcha => 
                (Solution)solution.ToObject<RecaptchaSolution>()!,
            CaptchaType.FunCaptcha => solution.ToObject<FuncaptchaSolution>(),
            CaptchaType.ImageCaptcha => solution.ToObject<ImageCaptchaSolution>(),
            CaptchaType.GeeTest => solution.ToObject<GeeTestSolution>(),
            CaptchaType.DataDome => solution.ToObject<DataDomeSolution>(),
            CaptchaType.CloudflareTurnstile => solution.ToObject<CloudflareTurnstileSolution>(),
            _ => throw new NotSupportedException($"The captcha type {task.Type} is not supported")
        } ?? throw new TaskSolutionException("The solution is null");

        return result.Solution.ToCaptchaResponse(task.Id) as T;
    }
    #endregion
    
    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolution(string id, CaptchaType type, bool correct = false, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostJsonAsync<CaptchaTaskFeedbackResponse>(
                "feedbackTask",
                new CaptchaTaskFeedbackRequest
                {
                    ClientKey = ApiKey,
                    AppId = _appId,
                    TaskId = id,
                    Result = new TaskResultFeedback
                    {
                        Invalid = !correct
                    }
                },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (response.IsError)
        {
            throw new TaskReportException($"{response.ErrorCode}: {response.ErrorDescription}");
        }
    }
    #endregion

    #region Private Methods
    private CaptchaTaskRequest CreateTaskRequest()
    {
        return new CaptchaTaskRequest
        {
            ClientKey = ApiKey,
            AppId = _appId
        };
    }
    #endregion

    #region Capabilities
    /// <inheritdoc/>
    public override CaptchaServiceCapabilities Capabilities =>
        CaptchaServiceCapabilities.None;
    #endregion
}
