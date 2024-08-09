using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Models;
using Newtonsoft.Json;

namespace CaptchaSharp.Extensions;

/// <summary>
/// Extensions for an <see cref="HttpClient"/>.
/// </summary>
public static class HttpClientExtensions
{
    /*
     * GET METHODS
     */

    /// <summary>
    /// Automatically builds a GET query string from a <see cref="StringPairCollection"/> 
    /// and appends it to the provided URL.
    /// </summary>
    public static async Task<HttpResponseMessage> GetAsync(
        this HttpClient httpClient, string url, StringPairCollection pairs,
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetAsync($"{url}?{pairs.ToHttpQueryString()}", cancellationToken)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Automatically builds a GET query string from a <see cref="StringPairCollection"/>
    /// and appends it to the provided URL. The response is then deserialized to the provided type.
    /// </summary>
    public static async Task<T> GetJsonAsync<T>(
        this HttpClient httpClient, string url, StringPairCollection pairs,
        CancellationToken cancellationToken = default) where T : notnull
    {
        using var response = await httpClient.GetAsync(url, pairs, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return json.Deserialize<T>();
    }

    /// <summary>
    /// Automatically builds a GET query string from a <see cref="StringPairCollection"/> 
    /// and appends it to the provided URL.
    /// </summary>
    public static async Task<string> GetStringAsync(
        this HttpClient httpClient, string url, StringPairCollection pairs,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(url, pairs, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    /*
     * POST METHODS
     */

    /// <summary>
    /// Automatically builds a POST query string from a <see cref="StringPairCollection"/> 
    /// using <see cref="Encoding.UTF8"/> encoding and the provided Content-Type.
    /// </summary>
    public static async Task<HttpResponseMessage> PostAsync(
        this HttpClient httpClient, string url, StringPairCollection pairs,
        string mediaType = "application/x-www-form-urlencoded",
        CancellationToken cancellationToken = default)
    {
        return await httpClient.PostAsync(url,
            new StringContent(pairs.ToHttpQueryString(), Encoding.UTF8, mediaType),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Automatically builds a POST query string from a <see cref="StringPairCollection"/> 
    /// using <see cref="Encoding.UTF8"/> encoding and the provided Content-Type.
    /// </summary>
    public static async Task<string> PostToStringAsync(
        this HttpClient httpClient, string url, StringPairCollection pairs,
        string mediaType = "application/x-www-form-urlencoded",
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync(url, pairs, mediaType, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request with the desired <see cref="MultipartFormDataContent"/> and reads the 
    /// response as a <see cref="string"/>.
    /// </summary>
    public static async Task<string> PostMultipartToStringAsync(
            this HttpClient httpClient, string url, MultipartFormDataContent content,
            CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Sends a POST request with the desired <see cref="MultipartFormDataContent"/> and reads the
    /// response as a <see cref="string"/>. The response is then deserialized to the provided type.
    /// </summary>
    public static async Task<T> PostMultipartAsync<T>(
        this HttpClient httpClient, string url, MultipartFormDataContent content,
        CancellationToken cancellationToken = default) where T : notnull
    {
        using var response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return json.Deserialize<T>();
    }

    /// <summary>
    /// Automatically builds a POST json string from a given object using <see cref="Encoding.UTF8"/> encoding 
    /// and application/json Content-Type.
    /// </summary>
    public static async Task<string> PostJsonToStringAsync(
        this HttpClient httpClient, string url, object content, bool camelizeKeys = true,
        CancellationToken cancellationToken = default)
    {
        var json = camelizeKeys 
            ? content.SerializeCamelCase()
            : JsonConvert.SerializeObject(content);

        using var response = await httpClient.PostAsync(url,
            new StringContent(json, Encoding.UTF8, "application/json"),
            cancellationToken).ConfigureAwait(false);
        
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Automatically builds a POST json string from a given object using <see cref="Encoding.UTF8"/> encoding
    /// </summary>
    public static async Task<HttpResponseMessage> PostJsonAsync(
        this HttpClient httpClient, string url, object content, bool camelizeKeys = true,
        CancellationToken cancellationToken = default)
    {
        var json = camelizeKeys
            ? content.SerializeCamelCase()
            : JsonConvert.SerializeObject(content);

        return await httpClient.PostAsync(url,
            new StringContent(json, Encoding.UTF8, "application/json"),
            cancellationToken).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Automatically builds a POST json string from a given object using <see cref="Encoding.UTF8"/> encoding
    /// and application/json Content-Type. The response is then deserialized to the provided type.
    /// </summary>
    public static async Task<T> PostJsonAsync<T>(
        this HttpClient httpClient, string url, object content, bool camelizeKeys = true,
        CancellationToken cancellationToken = default) where T : notnull
    {
        var json = camelizeKeys
            ? content.SerializeCamelCase()
            : JsonConvert.SerializeObject(content);

        using var response = await httpClient.PostAsync(url,
            new StringContent(json, Encoding.UTF8, "application/json"),
            cancellationToken).ConfigureAwait(false);

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return responseJson.Deserialize<T>();
    }
}
