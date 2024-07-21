using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.EzCaptcha.Responses;
using CaptchaSharp.Models.EzCaptcha.Responses.Solutions;
using CaptchaSharp.Models.EzCaptcha.Requests;
using CaptchaSharp.Models.EzCaptcha.Requests.Tasks;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services;

/// <summary>
/// The service offered by https://www.ez-captcha.com/
/// </summary>
public class EzCaptchaService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The ID of the software developer.
    /// </summary>
    private const int _softId = 82832;

    /// <summary>
    /// Initializes a <see cref="EzCaptchaService"/>.
    /// </summary>
    public EzCaptchaService(string apiKey, HttpClient? httpClient = null)
        : base(httpClient)
    {
        ApiKey = apiKey;
        HttpClient.BaseAddress = new Uri("https://api.ez-captcha.com");
    }
    
    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostJsonAsync<GetBalanceEzCaptchaResponse>(
            "getBalance", new EzCaptchaRequest { ClientKey = ApiKey },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (response.IsError)
        {
            throw new BadAuthenticationException($"{response.ErrorCode}: {response.ErrorDescription}");
        }

        return response.Balance;
    }
    #endregion
    
    #region Solve Methods
    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        // All recaptcha tasks are proxyless, so the proxy is disregarded
        
        var content = CreateTaskRequest();

        if (enterprise)
        {
            if (!string.IsNullOrEmpty(dataS))
            {
                content.Task = new ReCaptchaV2SEnterpriseTaskProxyless()
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    IsInvisible = invisible,
                    DataS = dataS,
                };
            }
            else
            {
                content.Task = new ReCaptchaV2EnterpriseTaskProxyless()
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    IsInvisible = invisible,
                };
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(dataS))
            {
                content.Task = new ReCaptchaV2STaskProxyless
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    IsInvisible = invisible,
                    DataS = dataS
                };
            }
            else
            {
                content.Task = new ReCaptchaV2TaskProxyless
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    IsInvisible = invisible
                };
            }
        }
            
        var response = await HttpClient.PostJsonAsync<TaskCreationEzCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(response, CaptchaType.ReCaptchaV2,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();

        if (enterprise)
        {
            content.Task = new ReCaptchaV3EnterpriseTaskProxyless()
            {
                WebsiteKey = siteKey,
                WebsiteURL = siteUrl,
                PageAction = string.IsNullOrEmpty(action) ? null : action,
            };
        }
        else
        {
            if (Math.Abs(minScore - 0.9f) < 0.000001)
            {
                content.Task = new ReCaptchaV3TaskProxylessS9()
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    PageAction = string.IsNullOrEmpty(action) ? null : action,
                };
            }
            else
            {
                content.Task = new ReCaptchaV3TaskProxyless()
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    PageAction = string.IsNullOrEmpty(action) ? null : action,
                };
            }
        }
        
        var response = await HttpClient.PostJsonAsync<TaskCreationEzCaptchaResponse>(
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
        var content = CreateTaskRequest();
        content.Task = new FuncaptchaTaskProxyless
        {
            WebsiteKey = publicKey,
            WebsiteURL = siteUrl,
            FuncaptchaApiJSSubdomain = serviceUrl
        };
        
        var response = await HttpClient.PostJsonAsync<TaskCreationEzCaptchaResponse>(
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
        content.Task = new HcaptchaTaskProxyless
        {
            WebsiteKey = siteKey,
            WebsiteURL = siteUrl
        };
        
        var response = await HttpClient.PostJsonAsync<TaskCreationEzCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(response, CaptchaType.HCaptcha,
            cancellationToken).ConfigureAwait(false);
    }
    #endregion
    
    #region Getting the result
    /// <summary>
    /// Gets the result of a task.
    /// </summary>
    private async Task<T> GetResult<T>(
        TaskCreationEzCaptchaResponse ezCaptchaResponse, CaptchaType type,
        CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (ezCaptchaResponse.IsError)
        {
            throw new TaskCreationException($"{ezCaptchaResponse.ErrorCode}: {ezCaptchaResponse.ErrorDescription}");
        }

        var task = new CaptchaTask(ezCaptchaResponse.TaskId!, type);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await HttpClient.PostJsonToStringAsync(
            "getTaskResult",
            new GetTaskResultEzCaptchaRequest { ClientKey = ApiKey, TaskId = task.Id },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var result = response.Deserialize<GetTaskResultEzCaptchaResponse>();

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
            throw new TaskSolutionException(response);
        }
        
        if (task.Type == CaptchaType.DataDome)
        {
            return ParseDataDomeSolution(task.Id, solution) as T;
        }

        result.EzCaptchaTaskSolution = task.Type switch
        {
            CaptchaType.ReCaptchaV2 or CaptchaType.ReCaptchaV3 or CaptchaType.HCaptcha => 
                solution.ToObject<RecaptchaEzCaptchaTaskSolution>()! as EzCaptchaTaskSolution,
            _ => throw new NotSupportedException($"The {task.Type} captcha type is not supported")
        } ?? throw new TaskSolutionException(response);

        return result.EzCaptchaTaskSolution.ToCaptchaResponse(task.Id) as T;
    }

    /// <summary>
    /// Parses the solution of a DataDome captcha.
    /// </summary>
    protected virtual StringResponse ParseDataDomeSolution(string taskId, JToken? solution)
    {
        throw new NotImplementedException("DataDome captcha solving is not supported");
    }
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Creates a new <see cref="CaptchaTaskEzCaptchaRequest"/>.
    /// </summary>
    private CaptchaTaskEzCaptchaRequest CreateTaskRequest()
    {
        return new CaptchaTaskEzCaptchaRequest
        {
            ClientKey = ApiKey,
            SoftId = _softId
        };
    }
    #endregion
}
