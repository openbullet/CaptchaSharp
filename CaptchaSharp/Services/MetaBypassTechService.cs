using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Enums;
using CaptchaSharp.Exceptions;
using CaptchaSharp.Extensions;
using CaptchaSharp.Models;
using CaptchaSharp.Models.MetaBypassTech;

namespace CaptchaSharp.Services;

/// <summary>
/// The service provided by https://metabypass.tech/
/// </summary>
public class MetaBypassTechService : CaptchaService
{
    /// <summary>
    /// The client ID to use.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// The client secret to use.
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// The username.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The password.
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// The current access token.
    /// </summary>
    private MbtAccessTokenResponse? _accessToken;

    /// <summary>
    /// Initializes a <see cref="MetaBypassTechService"/>.
    /// </summary>
    /// <param name="clientId">The client ID to use.</param>
    /// <param name="clientSecret">The client secret to use.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="httpClient">The <see cref="HttpClient"/> to use for requests. If null, a default one will be created.</param>
    public MetaBypassTechService(string clientId, string clientSecret,
        string username, string password, HttpClient? httpClient = null) : base(httpClient)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        Username = username;
        Password = password;
        
        HttpClient.BaseAddress = new Uri("https://app.metabypass.tech/CaptchaSolver/");

        // Since some captchas are returned directly in the response body,
        // we need to set a high timeout to account for those requests
        HttpClient.Timeout = Timeout;
    }
    
    #region Getting the Balance
    /// <inheritdoc />
    public override async Task<decimal> GetBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync().ConfigureAwait(false);
        
        var response = await HttpClient.GetJsonAsync<MbtResponse>(
                "api/v1/me",
                new StringPairCollection(),
                cancellationToken)
            .ConfigureAwait(false);
        
        if (!response.Ok)
        {
            throw new BadAuthenticationException(
                response.Message ?? "Unknown error");
        }

        return decimal.Parse(response.Data!["total_balance"]!.ToString());
    }
    #endregion
    
    #region Solve Methods
    /// <inheritdoc />
    public override async Task<StringResponse> SolveImageCaptchaAsync(
        string base64, ImageCaptchaOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureAccessTokenAsync().ConfigureAwait(false);

        var numeric = 0;
        
        if (options is not null)
        {
            numeric = options.CharacterSet switch
            {
                CharacterSet.OnlyNumbers => 1,
                CharacterSet.OnlyLetters => 2,
                CharacterSet.OnlyNumbersOrOnlyLetters => 3,
                CharacterSet.BothNumbersAndLetters => 4,
                _ => 0
            };
        }
        
        var payload = new MbtSolveImageCaptchaRequest
        {
            Base64Image = base64,
            Numeric = numeric,
            MinLength = options?.MinLength ?? 0,
            MaxLength = options?.MaxLength ?? 0
        };
        
        var response = await HttpClient.PostJsonAsync<MbtResponse>(
                "api/v1/services/captchaSolver",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        if (!response.Ok)
        {
            throw new TaskSolutionException(
                response.Message ?? "Unknown error");
        }
        
        return new StringResponse
        {
            Id = "0",
            Response = response.Data!["result"]!.ToString()
        };
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveRecaptchaV2Async(
        string siteKey, string siteUrl, string dataS = "", bool enterprise = false,
        bool invisible = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        if (proxy?.Host is not null)
        {
            throw new NotSupportedException("Proxies are not supported by metabypass.tech");
        }
        
        await EnsureAccessTokenAsync().ConfigureAwait(false);
        
        // When using version "invisible", we get "Service Failed" as a response
        // so we're just going to ignore it and use version "2" instead
        
        var payload = new MbtSolveRecaptchaRequest
        {
            SiteKey = siteKey,
            Url = siteUrl,
            Version = "2"
        };
        
        var response = await HttpClient.PostJsonAsync<MbtResponse>(
                "api/v1/services/bypassReCaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        if (!response.Ok)
        {
            throw new TaskSolutionException(
                response.Message ?? "Unknown error");
        }

        var captchaId = response.Data!["RecaptchaId"]!.ToString();

        return await GetResultAsync<StringResponse>(
            new CaptchaTask(captchaId, CaptchaType.ReCaptchaV2), 
            cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task<StringResponse> SolveRecaptchaV3Async(
        string siteKey, string siteUrl, string action = "verify", float minScore = 0.4f,
        bool enterprise = false, Proxy? proxy = null, CancellationToken cancellationToken = default)
    {
        if (proxy?.Host is not null)
        {
            throw new NotSupportedException("Proxies are not supported by metabypass.tech");
        }
        
        await EnsureAccessTokenAsync().ConfigureAwait(false);
        
        var payload = new MbtSolveRecaptchaRequest
        {
            SiteKey = siteKey,
            Url = siteUrl,
            Version = "3"
        };
        
        var response = await HttpClient.PostJsonAsync<MbtResponse>(
                "api/v1/services/bypassReCaptcha",
                payload,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        
        if (!response.Ok)
        {
            throw new TaskSolutionException(
                response.Message ?? "Unknown error");
        }
        
        return new StringResponse
        {
            Id = "0",
            Response = response.Data!["RecaptchaResponse"]!.ToString()
        };
    }

    #endregion
    
    #region Getting the result
    /// <inheritdoc />
    protected override async Task<T?> CheckResultAsync<T>(
        CaptchaTask task, CancellationToken cancellationToken = default) where T : class
    {
        if (task.Type is not CaptchaType.ReCaptchaV2)
        {
            throw new NotSupportedException(
                "The getCaptchaResult method is only supported for ReCaptchaV2 tasks");
        }
        
        var response = await HttpClient.GetJsonAsync<MbtResponse>(
            "api/v1/services/getCaptchaResult",
            new StringPairCollection()
                .Add("recaptcha_id", task.Id),
            cancellationToken).ConfigureAwait(false);
        
        if (!response.Ok)
        {
            throw new TaskSolutionException(
                response.Message ?? "Unknown error");
        }
        
        if (response.Data!["step"]!.ToString() == "pending")
        {
            return null;
        }

        return new StringResponse
        {
            Id = task.Id,
            Response = response.Data!["RecaptchaResponse"]!.ToString()
        } as T;
    }
    #endregion
    
    #region Private Methods
    private async ValueTask EnsureAccessTokenAsync()
    {
        if (_accessToken is null)
        {
            await GetAccessTokenAsync().ConfigureAwait(false);
            return;
        }

        if (_accessToken.ExpirationDate < DateTime.Now)
        {
            await RefreshAccessTokenAsync(_accessToken).ConfigureAwait(false);
        }
    }

    private async Task GetAccessTokenAsync()
    {
        var payload = new MbtAccessTokenRequest
        {
            GrantType = "password",
            ClientId = ClientId,
            ClientSecret = ClientSecret,
            Username = Username,
            Password = Password
        };
        
        using var response = await HttpClient.PostJsonAsync(
                "oauth/token",
                payload,
                cancellationToken: default)
            .ConfigureAwait(false);
        
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var serviceResponse = json.Deserialize<MbtResponse>();
            
            throw new BadAuthenticationException(
                serviceResponse.Message ?? "Unknown error");
        }
        
        _accessToken = json.Deserialize<MbtAccessTokenResponse>();
        HttpClient.DefaultRequestHeaders.Add("Authorization",
            $"{_accessToken.TokenType} {_accessToken.AccessToken}");
    }

    private async Task RefreshAccessTokenAsync(MbtAccessTokenResponse tokenResponse)
    {
        var payload = new MbtRefreshAccessTokenRequest
        {
            GrantType = "refresh_token",
            ClientId = ClientId,
            ClientSecret = ClientSecret,
            RefreshToken = tokenResponse.RefreshToken
        };
        
        using var response = await HttpClient.PostJsonAsync(
                "oauth/token",
                payload,
                cancellationToken: default)
            .ConfigureAwait(false);
        
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
        if (!response.IsSuccessStatusCode)
        {
            var serviceResponse = json.Deserialize<MbtResponse>();
            
            throw new BadAuthenticationException(
                serviceResponse.Message ?? "Unknown error");
        }
        
        _accessToken = json.Deserialize<MbtAccessTokenResponse>();
        HttpClient.DefaultRequestHeaders.Add("Authorization",
            $"{_accessToken.TokenType} {_accessToken.AccessToken}");
    }
    #endregion
}
