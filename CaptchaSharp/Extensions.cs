using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharp
{
    /// <summary>Extensions for an <see cref="HttpClient"/>.</summary>
    public static class HttpClientExtensions
    {
        /*
         * GET METHODS
         */

        /// <summary>Automatically builds a GET query string from a <see cref="StringPairCollection"/> 
        /// and appends it to the provided URL.</summary>
        public async static Task<HttpResponseMessage> GetAsync
            (this HttpClient httpClient, string url, StringPairCollection pairs,
            CancellationToken cancellationToken = default)
        {
            return await httpClient.GetAsync($"{url}?{pairs.ToHttpQueryString()}", cancellationToken);
        }

        /// <summary>Automatically builds a GET query string from a <see cref="StringPairCollection"/> 
        /// and appends it to the provided URL.</summary>
        /// <returns>The <see cref="HttpResponseMessage"/> content converted to a string.</returns>
        public async static Task<string> GetStringAsync
            (this HttpClient httpClient, string url, StringPairCollection pairs,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, pairs, cancellationToken);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /*
         * POST METHODS
         */

        /// <summary>Automatically builds a POST query string from a <see cref="StringPairCollection"/> 
        /// using <see cref="Encoding.UTF8"/> encoding and the provided Content-Type.</summary>
        public static async Task<HttpResponseMessage> PostAsync
            (this HttpClient httpClient, string url, StringPairCollection pairs,
            CancellationToken cancellationToken = default, string mediaType = "application/x-www-form-urlencoded")
        {
            return await httpClient.PostAsync(url,
                new StringContent(pairs.ToHttpQueryString(), Encoding.UTF8, mediaType),
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Automatically builds a POST query string from a <see cref="StringPairCollection"/> 
        /// using <see cref="Encoding.UTF8"/> encoding and the provided Content-Type.</summary>
        /// <returns>The <see cref="HttpResponseMessage"/> content converted to a <see cref="string"/>.</returns>
        public static async Task<string> PostToStringAsync
            (this HttpClient httpClient, string url, StringPairCollection pairs,
            CancellationToken cancellationToken = default, string mediaType = "application/x-www-form-urlencoded")
        {
            var response = await httpClient.PostAsync(url, pairs, cancellationToken, mediaType);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>Sends a POST request with the desired <see cref="MultipartFormDataContent"/> and reads the 
        /// response as a <see cref="string"/>.</summary>
        /// <returns>The <see cref="HttpResponseMessage"/> content converted to a <see cref="string"/>.</returns>
        public static async Task<string> PostMultipartToStringAsync
            (this HttpClient httpClient, string url, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>Automatically builds a POST json string from a given object using <see cref="Encoding.UTF8"/> encoding 
        /// and application/json Content-Type.</summary>
        /// <returns>The <see cref="HttpResponseMessage"/> content converted to a <see cref="string"/>.</returns>
        public static async Task<string> PostJsonToStringAsync<T>
            (this HttpClient httpClient, string url, T content, CancellationToken cancellationToken = default, bool camelizeKeys = true)
        {
            string json;

            if (camelizeKeys)
                json = content.SerializeCamelCase();
            else
                json = JsonConvert.SerializeObject(content);

            var response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Extensions for a <see cref="string"/>.</summary>
    public static class StringExtensions
    {
        /// <summary>Deserializes a json string to a given type.</summary>
        public static T Deserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>Serializes an object to a json string and converts the property names 
        /// to a camelCase based convention.</summary>
        public static string SerializeCamelCase<T>(this T obj)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(obj, settings);
        }

        /// <summary>Serializes an object to a json string and converts the property names 
        /// to a lowercase based convention.</summary>
        public static string SerializeLowerCase<T>(this T obj)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new LowercasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(obj, settings);
        }
    }

    /// <summary>Extensions for a <see cref="bool"/>.</summary>
    public static class BoolExtensions
    {
        /// <summary>Converts a bool to an int.</summary>
        /// <returns>0 if false, 1 if true.</returns>
        public static int ToInt(this bool boolean)
        {
            return boolean ? 1 : 0;
        }
    }

    /// <summary>Extensions for a <see cref="CaptchaLanguage"/>.</summary>
    public static class CaptchaLanguageExtensions
    {
        /// <summary>Converts a <see cref="CaptchaLanguage"/> to an ISO-639-1 country code.</summary>
        public static string ToISO6391Code(this CaptchaLanguage language)
        {
            var dict = new Dictionary<CaptchaLanguage, string>
            {
                { CaptchaLanguage.NotSpecified, "en" },
                { CaptchaLanguage.English,      "en" },
                { CaptchaLanguage.Russian,      "ru" },
                { CaptchaLanguage.Spanish,      "es" },
                { CaptchaLanguage.Portuguese,   "pt" },
                { CaptchaLanguage.Ukrainian,    "uk" },
                { CaptchaLanguage.Vietnamese,   "vi" },
                { CaptchaLanguage.French,       "fr" },
                { CaptchaLanguage.Indonesian,   "id" },
                { CaptchaLanguage.Arab,         "ar" },
                { CaptchaLanguage.Japanese,     "ja" },
                { CaptchaLanguage.Turkish,      "tr" },
                { CaptchaLanguage.German,       "de" },
                { CaptchaLanguage.Chinese,      "zh" },
                { CaptchaLanguage.Philippine,   "fil" },
                { CaptchaLanguage.Polish,       "pl" },
                { CaptchaLanguage.Thai,         "th" },
                { CaptchaLanguage.Italian,      "it" },
                { CaptchaLanguage.Dutch,        "nl" },
                { CaptchaLanguage.Slovak,       "sk" },
                { CaptchaLanguage.Bulgarian,    "bg" },
                { CaptchaLanguage.Romanian,     "ro" },
                { CaptchaLanguage.Hungarian,    "hu" },
                { CaptchaLanguage.Korean,       "ko" },
                { CaptchaLanguage.Czech,        "cs" },
                { CaptchaLanguage.Azerbaijani,  "az" },
                { CaptchaLanguage.Persian,      "fa" },
                { CaptchaLanguage.Bengali,      "bn" },
                { CaptchaLanguage.Greek,        "el" },
                { CaptchaLanguage.Lithuanian,   "lt" },
                { CaptchaLanguage.Latvian,      "lv" },
                { CaptchaLanguage.Swedish,      "sv" },
                { CaptchaLanguage.Serbian,      "sr" },
                { CaptchaLanguage.Croatian,     "hr" },
                { CaptchaLanguage.Hebrew,       "he" },
                { CaptchaLanguage.Hindi,        "hi" },
                { CaptchaLanguage.Norwegian,    "nb" },
                { CaptchaLanguage.Slovenian,    "sl" },
                { CaptchaLanguage.Danish,       "da" },
                { CaptchaLanguage.Uzbek,        "uz" },
                { CaptchaLanguage.Finnish,      "fi" },
                { CaptchaLanguage.Catalan,      "ca" },
                { CaptchaLanguage.Georgian,     "ka" },
                { CaptchaLanguage.Malay,        "ms" },
                { CaptchaLanguage.Telugu,       "te" },
                { CaptchaLanguage.Estonian,     "et" },
                { CaptchaLanguage.Malayalam,    "ml" },
                { CaptchaLanguage.Belorussian,  "be" },
                { CaptchaLanguage.Kazakh,       "kk" },
                { CaptchaLanguage.Marathi,      "mr" },
                { CaptchaLanguage.Nepali,       "ne" },
                { CaptchaLanguage.Burmese,      "my" },
                { CaptchaLanguage.Bosnian,      "bs" },
                { CaptchaLanguage.Armenian,     "hy" },
                { CaptchaLanguage.Macedonian,   "mk" },
                { CaptchaLanguage.Punjabi,      "pa" }
            };

            return dict[language];
        }
    }
}
