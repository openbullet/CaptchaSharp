using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Services.AntiCaptcha.Requests;
using CaptchaSharp.Services.AntiCaptcha.Requests.Tasks;
using CaptchaSharp.Services.AntiCaptcha.Requests.Tasks.Proxied;
using CaptchaSharp.Services.AntiCaptcha.Responses;
using CaptchaSharp.Services.AntiCaptcha.Responses.Solutions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by <c>https://anti-captcha.com/</c>
/// </summary>
public class AntiCaptchaService : CaptchaService
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
    /// The ID of the software developer.
    /// </summary>
    private const int _softId = 934;

    /// <summary>
    /// Initializes a <see cref="AntiCaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public AntiCaptchaService(string apiKey, HttpClient? httpClient = null)
    {
        ApiKey = apiKey;
        this._httpClient = httpClient ?? new HttpClient();
        this._httpClient.BaseAddress = new Uri("https://api.anti-captcha.com");
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostJsonToStringAsync(
            "getBalance", new Request { ClientKey = ApiKey },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var balanceResponse = response.Deserialize<GetBalanceResponse>();

        if (balanceResponse.IsError)
        {
            throw new BadAuthenticationException($"{balanceResponse.ErrorCode}: {balanceResponse.ErrorDescription}");
        }

        return new decimal(balanceResponse.Balance);
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
                AddImageCapabilities(
                    new CaptchaTaskRequest
                    {
                        ClientKey = ApiKey,
                        SoftId = _softId,
                        Task = new ImageCaptchaTask
                        {
                            Body = base64
                        }
                    }, options),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response.Deserialize<TaskCreationResponse>(), CaptchaType.ImageCaptcha,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();

        if (enterprise)
        {
            if (proxy != null)
            {
                content.Task = new RecaptchaV2EnterpriseTask
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    EnterprisePayload = new Dictionary<string, string>()
                }.SetProxy(proxy);

                if (!string.IsNullOrEmpty(dataS))
                {
                    ((RecaptchaV2EnterpriseTask)content.Task).EnterprisePayload.Add("s", dataS);
                }
            }
            else
            {
                content.Task = new RecaptchaV2EnterpriseTaskProxyless
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    EnterprisePayload = new Dictionary<string, string>()
                };

                if (!string.IsNullOrEmpty(dataS))
                {
                    ((RecaptchaV2EnterpriseTaskProxyless)content.Task).EnterprisePayload.Add("s", dataS);
                }
            }
        }
        else
        {
            if (proxy != null)
            {
                content.Task = new RecaptchaV2Task
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    IsInvisible = invisible
                }.SetProxy(proxy);
            }
            else
            {
                content.Task = new RecaptchaV2TaskProxyless
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    IsInvisible = invisible
                };
            }
        }
            
        var response = await _httpClient.PostJsonToStringAsync
            ("createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response.Deserialize<TaskCreationResponse>(), CaptchaType.ReCaptchaV2,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f, bool enterprise = false,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        if (proxy != null)
        {
            throw new NotSupportedException("Proxies are not supported");
        }

        if (minScore != 0.3f && minScore != 0.7f && minScore != 0.9f)
        {
            throw new NotSupportedException("Only min scores of 0.3, 0.7 and 0.9 are supported");
        }

        var content = CreateTaskRequest();

        content.Task = new RecaptchaV3TaskProxyless
        {
            WebsiteKey = siteKey,
            WebsiteURL = siteUrl,
            PageAction = action,
            MinScore = minScore,
            IsEnterprise = enterprise
        };

        var response = await _httpClient.PostJsonToStringAsync(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response.Deserialize<TaskCreationResponse>(), CaptchaType.ReCaptchaV3,
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

        if (proxy != null)
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

        var response = await _httpClient.PostJsonToStringAsync(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response.Deserialize<TaskCreationResponse>(), CaptchaType.FunCaptcha, 
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, Proxy? proxy = null, 
        CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
            
        if (proxy != null)
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
            
        var response = await _httpClient.PostJsonToStringAsync(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<StringResponse>(
            response.Deserialize<TaskCreationResponse>(), CaptchaType.HCaptcha,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null,
        Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
            
        if (proxy != null)
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
                GeetestApiServerSubdomain = apiServer
            };
        }

        var response = await _httpClient.PostJsonToStringAsync
            ("createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResult<GeeTestResponse>(
            response.Deserialize<TaskCreationResponse>(), CaptchaType.GeeTest,
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
            new GetTaskResultRequest { ClientKey = ApiKey, TaskId = (int)task.Id },
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
            throw new TaskSolutionException(response);
        }

        result.Solution = task.Type switch
        {
            CaptchaType.ReCaptchaV2 or CaptchaType.ReCaptchaV3 or CaptchaType.HCaptcha => 
                solution.ToObject<RecaptchaSolution>()! as Solution,
            CaptchaType.FunCaptcha => solution.ToObject<FuncaptchaSolution>()!,
            CaptchaType.ImageCaptcha => solution.ToObject<ImageCaptchaSolution>(),
            CaptchaType.GeeTest => solution.ToObject<GeeTestSolution>(),
            _ => throw new NotSupportedException($"The {task.Type} captcha type is not supported")
        } ?? throw new TaskSolutionException(response);

        return result.Solution.ToCaptchaResponse(task.Id) as T;
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolution(
        long id, CaptchaType type, bool correct = false,
        CancellationToken cancellationToken = default)
    {
        if (correct)
        {
            throw new NotSupportedException("This service doesn't allow reporting of good solutions");
        }

        string response;
        ReportIncorrectCaptchaResponse incResponse;

        switch (type)
        {
            case CaptchaType.ImageCaptcha:
                response = await _httpClient.PostJsonToStringAsync(
                    "reportIncorrectImageCaptcha",
                    new ReportIncorrectCaptchaRequest { ClientKey = ApiKey, TaskId = id },
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                incResponse = response.Deserialize<ReportIncorrectCaptchaResponse>();
                break;

            case CaptchaType.ReCaptchaV2:
            case CaptchaType.ReCaptchaV3:
                response = await _httpClient.PostJsonToStringAsync(
                    "reportIncorrectRecaptcha",
                    new ReportIncorrectCaptchaRequest { ClientKey = ApiKey, TaskId = id },
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                incResponse = response.Deserialize<ReportIncorrectCaptchaResponse>();
                break;

            default:
                throw new NotSupportedException("Reporting is not supported for this captcha type");
        }

        if (incResponse.NotFoundOrExpired)
        {
            throw new TaskReportException("Captcha not found or expired");
        }
    }
    #endregion

    #region Private Methods
    private CaptchaTaskRequest CreateTaskRequest()
    {
        return new CaptchaTaskRequest
        {
            ClientKey = ApiKey,
            SoftId = _softId
        };
    }
    #endregion

    #region Capabilities
    /// <inheritdoc/>
    public override CaptchaServiceCapabilities Capabilities =>
        CaptchaServiceCapabilities.Language |
        CaptchaServiceCapabilities.Phrases |
        CaptchaServiceCapabilities.CaseSensitivity |
        CaptchaServiceCapabilities.CharacterSets |
        CaptchaServiceCapabilities.Calculations |
        CaptchaServiceCapabilities.MinLength |
        CaptchaServiceCapabilities.MaxLength |
        CaptchaServiceCapabilities.Instructions;

    private static CaptchaTaskRequest AddImageCapabilities(CaptchaTaskRequest request, ImageCaptchaOptions? options)
    {
        if (options == null)
        {
            return request;
        }

        if (request.Task is not ImageCaptchaTask task)
        {
            throw new NotSupportedException(
                "Image options are only supported for image captchas");
        }

        task.Phrase = options.IsPhrase;
        task.Case = options.CaseSensitive;

        task.Numeric = options.CharacterSet switch
        {
            CharacterSet.OnlyNumbers => 1,
            CharacterSet.OnlyLetters => 2,
            _ => 0
        };

        task.Math = options.RequiresCalculation;
        task.MinLength = options.MinLength;
        task.MaxLength = options.MaxLength;
        task.Comment = options.TextInstructions;

        request.LanguagePool = options.CaptchaLanguage switch
        {
            CaptchaLanguage.NotSpecified or CaptchaLanguage.English => "en",
            CaptchaLanguage.Russian or CaptchaLanguage.Ukrainian or CaptchaLanguage.Kazakh
                or CaptchaLanguage.Belorussian => "rn",
            _ => throw new NotSupportedException($"The {options.CaptchaLanguage} language is not supported")
        };

        return request;
    }
    #endregion
}
