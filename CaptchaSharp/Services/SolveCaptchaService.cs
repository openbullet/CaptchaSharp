using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.SolveCaptcha.Requests;
using CaptchaSharp.Models.SolveCaptcha.Requests.Tasks;
using CaptchaSharp.Models.SolveCaptcha.Requests.Tasks.Proxied;
using CaptchaSharp.Models.SolveCaptcha.Responses;
using CaptchaSharp.Models.SolveCaptcha.Responses.Solutions;
using Newtonsoft.Json.Linq;

namespace CaptchaSharp.Services;

/// <summary>
/// The service offered by https://solvecaptcha.net/
/// </summary>
public class SolveCaptchaService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The ID of the software developer.
    /// </summary>
    private const string _affiliateId = "1f9531ae-f170-4beb-9148-09da563df4bd";
    
    /// <summary>
    /// Initializes a <see cref="SolveCaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public SolveCaptchaService(string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        ApiKey = apiKey;
        HttpClient.BaseAddress = new Uri("https://solvecaptcha.net/api/");
        HttpClient.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", apiKey);
    }
    
    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.GetJsonAsync<GetBalanceSolveCaptchaResponse>(
            "getBalance", new StringPairCollection(),
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
        var content = CreateTaskRequest();

        if (enterprise)
        {   
            if (!string.IsNullOrEmpty(proxy?.Host))
            {
                content.Task = new RecaptchaV2EnterpriseTask
                {
                    SiteKey = siteKey,
                    PageUrl = siteUrl,
                    EnterprisePayload = string.IsNullOrEmpty(dataS)
                        ? null
                        : $"{{\"s\":\"{dataS}\"}}"
                };
            }
            else
            {
                content.Task = new RecaptchaV2EnterpriseTaskProxyless
                {
                    SiteKey = siteKey,
                    PageUrl = siteUrl,
                    EnterprisePayload = string.IsNullOrEmpty(dataS)
                        ? null
                        : $"{{\"s\":\"{dataS}\"}}"
                };
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(proxy?.Host))
            {
                content.Task = new RecaptchaV2Task
                {
                    SiteKey = siteKey,
                    PageUrl = siteUrl,
                    DataS = string.IsNullOrEmpty(dataS) ? null : dataS
                };
            }
            else
            {
                content.Task = new RecaptchaV2TaskProxyless
                {
                    SiteKey = siteKey,
                    PageUrl = siteUrl,
                    DataS = string.IsNullOrEmpty(dataS) ? null : dataS
                };
            }
        }
            
        content.Task.SetParamsFromProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<TaskCreationSolveCaptchaResponse>(
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
        content.Task = new RecaptchaV3TaskProxyless
        {
            SiteKey = siteKey,
            PageUrl = siteUrl,
            PageAction = string.IsNullOrEmpty(action) ? null : action,
            MinScore = minScore
        };
        
        content.Task.SetParamsFromProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<TaskCreationSolveCaptchaResponse>(
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

        if (!string.IsNullOrEmpty(proxy?.Host))
        {
            content.Task = new FunCaptchaTask
            {
                SiteKey = publicKey,
                SubDomain = serviceUrl,
                PageUrl = siteUrl,
            };
        }
        else
        {
            content.Task = new FunCaptchaTaskProxyless
            {
                SiteKey = publicKey,
                SubDomain = serviceUrl,
                PageUrl = siteUrl,
            };
        }
        
        content.Task.SetParamsFromProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<TaskCreationSolveCaptchaResponse>(
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
        
        if (!string.IsNullOrEmpty(proxy?.Host))
        {
            content.Task = new HCaptchaTask
            {
                SiteKey = siteKey,
                PageUrl = siteUrl
            };
        }
        else
        {
            content.Task = new HCaptchaTaskProxyless
            {
                SiteKey = siteKey,
                PageUrl = siteUrl
            };
        }
        
        content.Task.SetParamsFromProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<TaskCreationSolveCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<StringResponse>(response, CaptchaType.HCaptcha,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null, Proxy? proxy = null,
        CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
        
        if (!string.IsNullOrEmpty(proxy?.Host))
        {
            content.Task = new GeeTestTask
            {
                Gt = gt,
                Challenge = challenge,
                PageUrl = siteUrl,
                GeeTestApiServerSubdomain = apiServer
            };
        }
        else
        {
            content.Task = new GeeTestTaskProxyless
            {
                Gt = gt,
                Challenge = challenge,
                PageUrl = siteUrl,
                GeeTestApiServerSubdomain = apiServer
            };
        }
        
        content.Task.SetParamsFromProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<TaskCreationSolveCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<GeeTestResponse>(response, CaptchaType.GeeTest,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
        
        if (!string.IsNullOrEmpty(proxy?.Host))
        {
            content.Task = new TurnstileTask
            {
                SiteKey = siteKey,
                PageUrl = siteUrl,
            };
        }
        else
        {
            content.Task = new TurnstileTaskProxyless
            {
                SiteKey = siteKey,
                PageUrl = siteUrl,
            };
        }
        
        content.Task.SetParamsFromProxy(proxy);
        
        var response = await HttpClient.PostJsonAsync<TaskCreationSolveCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        return await GetResult<CloudflareTurnstileResponse>(response, CaptchaType.CloudflareTurnstile,
            cancellationToken).ConfigureAwait(false);
    }
    #endregion
    
    #region Getting the result
    /// <summary>
    /// Gets the result of a task.
    /// </summary>
    private async Task<T> GetResult<T>(
        TaskCreationSolveCaptchaResponse antiCaptchaResponse, CaptchaType type,
        CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (antiCaptchaResponse.IsError)
        {
            throw new TaskCreationException($"{antiCaptchaResponse.ErrorCode}: {antiCaptchaResponse.ErrorDescription}");
        }

        var task = new CaptchaTask(antiCaptchaResponse.TaskId.ToString(), type);

        return await GetResult<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResult<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await HttpClient.PostJsonToStringAsync(
            "getTaskResult",
            new GetTaskResultSolveCaptchaRequest { TaskId = long.Parse(task.Id) },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Sometimes the API returns an empty json array
        if (response == "[]")
        {
            throw new TaskSolutionException("The API returned an empty json array");
        }

        GetTaskResultSolveCaptchaResponse result;

        try
        {
            result = response.Deserialize<GetTaskResultSolveCaptchaResponse>();
        }
        catch (Exception e)
        {
            throw new TaskSolutionException("The API returned an invalid json", e);
        }

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

        result.SolveCaptchaTaskSolution = task.Type switch
        {
            CaptchaType.ReCaptchaV2 or CaptchaType.ReCaptchaV3 => 
                solution.ToObject<RecaptchaSolveCaptchaTaskSolution>()! as SolveCaptchaTaskSolution,
            CaptchaType.FunCaptcha or CaptchaType.CloudflareTurnstile => 
                solution.ToObject<FuncaptchaSolveCaptchaTaskSolution>()!,
            CaptchaType.GeeTest => solution.ToObject<GeeTestSolveCaptchaTaskSolution>(),
            CaptchaType.HCaptcha => solution.ToObject<HCaptchaSolveCaptchaTaskSolution>(),
            _ => throw new NotSupportedException($"The {task.Type} captcha type is not supported")
        } ?? throw new TaskSolutionException(response);

        return result.SolveCaptchaTaskSolution.ToCaptchaResponse(task.Id) as T;
    }
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Creates a new <see cref="CaptchaTaskSolveCaptchaRequest"/>.
    /// </summary>
    private CaptchaTaskSolveCaptchaRequest CreateTaskRequest()
    {
        return new CaptchaTaskSolveCaptchaRequest
        {
            AffiliateId = _affiliateId
        };
    }
    #endregion
}
