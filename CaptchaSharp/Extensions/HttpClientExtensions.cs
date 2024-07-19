using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CaptchaSharp.Models;
using Newtonsoft.Json;

namespace CaptchaSharp.Extensions;

/// <summary>Extensions for an <see cref="HttpClient"/>.</summary>
public static class HttpClientExtensions
{
    /*
     * GET METHODS
     */

    /// <summary>Automatically builds a GET query string from a <see cref="StringPairCollection"/> 
    /// and appends it to the provided URL.</summary>
    public static async Task<HttpResponseMessage> GetAsync(
        this HttpClient httpClient, string url, StringPairCollection pairs,
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetAsync($"{url}?{pairs.ToHttpQueryString()}", cancellationToken);
    }

    /// <summary>Automatically builds a GET query string from a <see cref="StringPairCollection"/> 
    /// and appends it to the provided URL.</summary>
    /// <returns>The <see cref="HttpResponseMessage"/> content converted to a string.</returns>
    public static async Task<string> GetStringAsync(
        this HttpClient httpClient, string url, StringPairCollection pairs,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(url, pairs, cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    /*
     * POST METHODS
     */

    /// <summary>Automatically builds a POST query string from a <see cref="StringPairCollection"/> 
    /// using <see cref="Encoding.UTF8"/> encoding and the provided Content-Type.</summary>
    public static async Task<HttpResponseMessage> PostAsync(
        this HttpClient httpClient, string url, StringPairCollection pairs,
        string mediaType = "application/x-www-form-urlencoded",
        CancellationToken cancellationToken = default)
    {
        return await httpClient.PostAsync(url,
            new StringContent(pairs.ToHttpQueryString(), Encoding.UTF8, mediaType),
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Automatically builds a POST query string from a <see cref="StringPairCollection"/> 
    /// using <see cref="Encoding.UTF8"/> encoding and the provided Content-Type.</summary>
    /// <returns>The <see cref="HttpResponseMessage"/> content converted to a <see cref="string"/>.</returns>
    public static async Task<string> PostToStringAsync(
        this HttpClient httpClient, string url, StringPairCollection pairs,
        string mediaType = "application/x-www-form-urlencoded",
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync(url, pairs, mediaType, cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Sends a POST request with the desired <see cref="MultipartFormDataContent"/> and reads the 
    /// response as a <see cref="string"/>.</summary>
    /// <returns>The <see cref="HttpResponseMessage"/> content converted to a <see cref="string"/>.</returns>
    public static async Task<string> PostMultipartToStringAsync(
            this HttpClient httpClient, string url, MultipartFormDataContent content,
            CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }
    
    /// <summary>
    /// Automatically builds a POST json string from a given object using <see cref="Encoding.UTF8"/> encoding
    /// </summary>
    public static async Task<HttpResponseMessage> PostJsonAsync<T>(
        this HttpClient httpClient, string url, T content, bool camelizeKeys = true,
        CancellationToken cancellationToken = default) where T : class
    {
        var json = camelizeKeys 
            ? content.SerializeCamelCase()
            : JsonConvert.SerializeObject(content);

        return await httpClient.PostAsync(url,
            new StringContent(json, Encoding.UTF8, "application/json"),
            cancellationToken);
    }

    /// <summary>Automatically builds a POST json string from a given object using <see cref="Encoding.UTF8"/> encoding 
    /// and application/json Content-Type.</summary>
    /// <returns>The <see cref="HttpResponseMessage"/> content converted to a <see cref="string"/>.</returns>
    public static async Task<string> PostJsonToStringAsync<T>(
        this HttpClient httpClient, string url, T content, bool camelizeKeys = true,
        CancellationToken cancellationToken = default) where T : class
    {
        var json = camelizeKeys 
            ? content.SerializeCamelCase()
            : JsonConvert.SerializeObject(content);

        var response = await httpClient.PostAsync(url,
            new StringContent(json, Encoding.UTF8, "application/json"),
            cancellationToken);
        
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }
}
