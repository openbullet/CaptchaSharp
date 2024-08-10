using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.AntiCaptcha.Requests;
using CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;
using CaptchaSharp.Models.AntiCaptcha.Requests.Tasks.Proxied;
using CaptchaSharp.Models.AntiCaptcha.Responses;
using CaptchaSharp.Models.AntiCaptcha.Responses.Solutions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models.CaptchaOptions;
using CaptchaSharp.Models.CaptchaResponses;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://anti-captcha.com/
/// </summary>
public class AntiCaptchaService : CaptchaService
{
    /// <summary>
    /// Your secret api key.
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The ID of the software developer.
    /// </summary>
    protected int? SoftId = 934;

    /// <summary>
    /// Initializes a <see cref="AntiCaptchaService"/>.
    /// </summary>
    /// <param name="apiKey">Your secret api key.</param>
    /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public AntiCaptchaService(string apiKey, HttpClient? httpClient = null) : base(httpClient)
    {
        ApiKey = apiKey;
        this.HttpClient.BaseAddress = new Uri("https://api.anti-captcha.com");
    }

    #region Getting the Balance
    /// <inheritdoc/>
    public override async Task<decimal> GetBalanceAsync(CancellationToken cancellationToken = default)
    {
        var response = await HttpClient.PostJsonAsync<GetBalanceAntiCaptchaResponse>(
            "getBalance", new AntiCaptchaRequest { ClientKey = ApiKey },
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
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(base64))
        {
            throw new ArgumentException("The image base64 string is null or empty", nameof(base64));
        }
        
        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                AddImageCapabilities(
                    new CaptchaTaskAntiCaptchaRequest
                    {
                        ClientKey = ApiKey,
                        SoftId = SoftId,
                        Task = new ImageCaptchaTask
                        {
                            Body = base64
                        }
                    }, options),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<StringResponse>(response, CaptchaType.ImageCaptcha,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false, bool invisible = false,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();

        if (enterprise)
        {
            if (sessionParams?.Proxy is not null)
            {
                content.Task = new RecaptchaV2EnterpriseTask
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    EnterprisePayload = new Dictionary<string, string>()
                }.WithSessionParams(sessionParams);

                if (!string.IsNullOrEmpty(dataS))
                {
                    ((RecaptchaV2EnterpriseTask)content.Task).EnterprisePayload?.Add("s", dataS);
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
                    ((RecaptchaV2EnterpriseTaskProxyless)content.Task).EnterprisePayload?.Add("s", dataS);
                }
            }
        }
        else
        {
            if (sessionParams?.Proxy is not null)
            {
                content.Task = new RecaptchaV2Task
                {
                    WebsiteKey = siteKey,
                    WebsiteURL = siteUrl,
                    IsInvisible = invisible
                }.WithSessionParams(sessionParams);
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
            
        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<StringResponse>(response, CaptchaType.ReCaptchaV2,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f, bool enterprise = false,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        // Only min scores of 0.3, 0.7 and 0.9 are supported

        var content = CreateTaskRequest();

        content.Task = new RecaptchaV3TaskProxyless
        {
            WebsiteKey = siteKey,
            WebsiteURL = siteUrl,
            PageAction = action,
            MinScore = minScore,
            IsEnterprise = enterprise
        };

        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<StringResponse>(response, CaptchaType.ReCaptchaV3,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveFuncaptchaAsync(
        string publicKey, string serviceUrl, string siteUrl, bool noJs = false,
        string? data = null, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        if (noJs)
        {
            throw new NotSupportedException("This service does not support no js solving");
        }

        var content = CreateTaskRequest();

        if (sessionParams?.Proxy is not null)
        {
            content.Task = new FunCaptchaTask
            {
                WebsitePublicKey = publicKey,
                WebsiteUrl = siteUrl,
                FuncaptchaApiJsSubdomain = serviceUrl,
                Data = data
            }.WithSessionParams(sessionParams);
        }
        else
        {
            content.Task = new FunCaptchaTaskProxyless
            {
                WebsitePublicKey = publicKey,
                WebsiteUrl = siteUrl,
                FuncaptchaApiJsSubdomain = serviceUrl,
                Data = data
            };
        }

        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<StringResponse>(response, CaptchaType.FunCaptcha, 
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<StringResponse> SolveHCaptchaAsync(
        string siteKey, string siteUrl, bool invisible = false, string? enterprisePayload = null,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
            
        if (sessionParams?.Proxy is not null)
        {
            content.Task = new HCaptchaTask
            {
                WebsiteKey = siteKey,
                WebsiteUrl = siteUrl,
                IsInvisible = invisible,
                IsEnterprise = !string.IsNullOrEmpty(enterprisePayload),
                EnterprisePayload = enterprisePayload is null ? null : JObject.Parse(enterprisePayload)
            }.WithSessionParams(sessionParams);
        }
        else
        {
            content.Task = new HCaptchaTaskProxyless
            {
                WebsiteKey = siteKey,
                WebsiteUrl = siteUrl,
                IsInvisible = invisible,
                IsEnterprise = !string.IsNullOrEmpty(enterprisePayload),
                EnterprisePayload = enterprisePayload is null ? null : JObject.Parse(enterprisePayload)
            };
        }
            
        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<StringResponse>(response, CaptchaType.HCaptcha,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestResponse> SolveGeeTestAsync(
        string gt, string challenge, string siteUrl, string? apiServer = null,
        SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
            
        if (sessionParams?.Proxy is not null)
        {
            content.Task = new GeeTestTask
            {
                WebsiteURL = siteUrl,
                Gt = gt,
                Challenge = challenge,
                GeetestApiServerSubdomain = apiServer
            }.WithSessionParams(sessionParams);
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

        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<GeeTestResponse>(response, CaptchaType.GeeTest,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<CloudflareTurnstileResponse> SolveCloudflareTurnstileAsync(
        string siteKey, string siteUrl, string? action = null, string? data = null,
        string? pageData = null, SessionParams? sessionParams = null, CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
            
        if (sessionParams?.Proxy is not null)
        {
            content.Task = new TurnstileTask
            {
                WebsiteKey = siteKey,
                WebsiteURL = siteUrl,
                Action = action,
                TurnstileCData = data
            }.WithSessionParams(sessionParams);
        }
        else
        {
            content.Task = new TurnstileTaskProxyless
            {
                WebsiteKey = siteKey,
                WebsiteURL = siteUrl,
                Action = action,
                TurnstileCData = data
            };
        }
            
        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<CloudflareTurnstileResponse>(response, CaptchaType.CloudflareTurnstile,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async Task<GeeTestV4Response> SolveGeeTestV4Async(
        string captchaId, string siteUrl, SessionParams? sessionParams = null,
        CancellationToken cancellationToken = default)
    {
        var content = CreateTaskRequest();
            
        if (sessionParams?.Proxy is not null)
        {
            content.Task = new GeeTestTask
            {
                Gt = captchaId,
                WebsiteURL = siteUrl,
                Version = 4
            }.WithSessionParams(sessionParams);
        }
        else
        {
            content.Task = new GeeTestTaskProxyless
            {
                Gt = captchaId,
                WebsiteURL = siteUrl,
                Version = 4
            };
        }
            
        var response = await HttpClient.PostJsonAsync<TaskCreationAntiCaptchaResponse>(
                "createTask",
                content,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await GetResultAsync<GeeTestV4Response>(response, CaptchaType.GeeTestV4,
            cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Getting the result
    /// <summary>
    /// Gets the result of a task.
    /// </summary>
    protected async Task<T> GetResultAsync<T>(
        TaskCreationAntiCaptchaResponse antiCaptchaResponse, CaptchaType type,
        CancellationToken cancellationToken = default)
        where T : CaptchaResponse
    {
        if (antiCaptchaResponse.IsError)
        {
            throw new TaskCreationException($"{antiCaptchaResponse.ErrorCode}: {antiCaptchaResponse.ErrorDescription}");
        }

        var task = new CaptchaTask(antiCaptchaResponse.TaskId.ToString(), type);

        return await GetResultAsync<T>(task, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override async Task<T?> CheckResultAsync<T>(
        CaptchaTask task, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await HttpClient.PostJsonToStringAsync(
            "getTaskResult",
            new GetTaskResultAntiCaptchaRequest { ClientKey = ApiKey, TaskId = int.Parse(task.Id) },
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var result = response.Deserialize<GetTaskResultAntiCaptchaResponse>();

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
        
        if (task.Type == CaptchaType.CloudflareChallengePage)
        {
            return ParseCloudflareChallengePageSolution(task.Id, solution) as T;
        }

        result.AntiCaptchaTaskSolution = task.Type switch
        {
            CaptchaType.ReCaptchaV2 or CaptchaType.ReCaptchaV3 or CaptchaType.HCaptcha or CaptchaType.ReCaptchaMobile => 
                solution.ToObject<RecaptchaAntiCaptchaTaskSolution>()! as AntiCaptchaTaskSolution,
            CaptchaType.FunCaptcha => solution.ToObject<FuncaptchaAntiCaptchaTaskSolution>()!,
            CaptchaType.ImageCaptcha => solution.ToObject<ImageCaptchaAntiCaptchaTaskSolution>(),
            CaptchaType.GeeTest => solution.ToObject<GeeTestAntiCaptchaTaskSolution>(),
            CaptchaType.CloudflareTurnstile => solution.ToObject<TurnstileAntiCaptchaTaskSolution>(),
            CaptchaType.GeeTestV4 => solution.ToObject<GeeTestV4AntiCaptchaTaskSolution>(),
            _ => throw new NotSupportedException($"The {task.Type} captcha type is not supported")
        } ?? throw new TaskSolutionException(response);

        return result.AntiCaptchaTaskSolution.ToCaptchaResponse(task.Id) as T;
    }

    /// <summary>
    /// Parses the solution of a DataDome captcha.
    /// </summary>
    protected virtual StringResponse ParseDataDomeSolution(string taskId, JToken? solution)
    {
        throw new NotImplementedException("DataDome captcha solving is not supported");
    }
    
    protected virtual StringResponse ParseCloudflareChallengePageSolution(string taskId, JToken? solution)
    {
        throw new NotImplementedException("Cloudflare challenge page solving is not supported");
    }
    #endregion

    #region Reporting the solution
    /// <inheritdoc/>
    public override async Task ReportSolutionAsync(
        string id, CaptchaType type, bool correct = false,
        CancellationToken cancellationToken = default)
    {
        if (correct)
        {
            if (type is not CaptchaType.ReCaptchaV2 && type is not CaptchaType.ReCaptchaV3)
            {
                throw new NotSupportedException(
                    "Reporting correct solutions is only supported for ReCaptchaV2 and ReCaptchaV3");
            }
            
            await HttpClient.PostJsonToStringAsync(
                "reportCorrectRecaptcha",
                new ReportIncorrectAntiCaptchaRequest { ClientKey = ApiKey, TaskId = int.Parse(id) },
                cancellationToken: cancellationToken).ConfigureAwait(false);
            
            return;
        }

        var incAntiCaptchaResponse = type switch
        {
            CaptchaType.ImageCaptcha => await HttpClient.PostJsonAsync<ReportIncorrectCaptchaAntiCaptchaResponse>(
                    "reportIncorrectImageCaptcha",
                    new ReportIncorrectAntiCaptchaRequest { ClientKey = ApiKey, TaskId = int.Parse(id) },
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
            CaptchaType.ReCaptchaV2 or CaptchaType.ReCaptchaV3 => await HttpClient
                .PostJsonAsync<ReportIncorrectCaptchaAntiCaptchaResponse>("reportIncorrectRecaptcha",
                    new ReportIncorrectAntiCaptchaRequest { ClientKey = ApiKey, TaskId = int.Parse(id) },
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
            CaptchaType.HCaptcha => await HttpClient.PostJsonAsync<ReportIncorrectCaptchaAntiCaptchaResponse>(
                    "reportIncorrectHcaptcha",
                    new ReportIncorrectAntiCaptchaRequest { ClientKey = ApiKey, TaskId = int.Parse(id) },
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false),
            _ => throw new NotSupportedException("Reporting is not supported for this captcha type")
        };

        if (incAntiCaptchaResponse.NotFoundOrExpired)
        {
            throw new TaskReportException("Captcha not found or expired");
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Creates a new <see cref="CaptchaTaskAntiCaptchaRequest"/>.
    /// </summary>
    protected CaptchaTaskAntiCaptchaRequest CreateTaskRequest()
    {
        return new CaptchaTaskAntiCaptchaRequest
        {
            ClientKey = ApiKey,
            SoftId = SoftId
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

    private static CaptchaTaskAntiCaptchaRequest AddImageCapabilities(CaptchaTaskAntiCaptchaRequest antiCaptchaRequest, ImageCaptchaOptions? options)
    {
        if (options == null)
        {
            return antiCaptchaRequest;
        }

        if (antiCaptchaRequest.Task is not ImageCaptchaTask task)
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

        antiCaptchaRequest.LanguagePool = options.CaptchaLanguage switch
        {
            CaptchaLanguage.NotSpecified or CaptchaLanguage.English => "en",
            CaptchaLanguage.Russian or CaptchaLanguage.Ukrainian or CaptchaLanguage.Kazakh
                or CaptchaLanguage.Belorussian => "rn",
            _ => throw new NotSupportedException($"The {options.CaptchaLanguage} language is not supported")
        };

        return antiCaptchaRequest;
    }
    #endregion
}
