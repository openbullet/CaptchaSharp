using CaptchaSharp.Enums;
using CaptchaSharp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CaptchaSharp
{
    public static class BitmapExtensions
    {
        /// <summary>
        /// Gets the base64-encoded string from an image.
        /// </summary>
        /// <param name="image">The bitmap image</param>
        /// <param name="format">The format of the bitmap image</param>
        /// <returns>The base64-encoded string</returns>
        public static string ToBase64(this Bitmap image, ImageFormat format)
        {
            var imageBytes = image.ToStream(format).ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        // <summary>
        /// Gets a memory stream from an image.
        /// </summary>
        /// <param name="image">The bitmap image</param>
        /// <param name="format">The format of the bitmap image</param>
        /// <returns>The memory stream</returns>
        public static MemoryStream ToStream(this Bitmap image, ImageFormat format)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Gets the bytes from an image.
        /// </summary>
        /// <param name="image">The bitmap image</param>
        /// <returns>The bytes of the image</returns>
        public static byte[] ToBytes(this Bitmap image, ImageFormat format)
        {
            return Convert.FromBase64String(image.ToBase64(format));
        }
    }

    public static class HttpClientExtensions
    {
        // GET METHODS
        public async static Task<HttpResponseMessage> GetAsync
            (this HttpClient httpClient, string url, StringPairCollection pairs,
            CancellationToken cancellationToken = default)
        {
            return await httpClient.GetAsync($"{url}?{pairs.ToHttpQueryString()}", cancellationToken);
        }

        public async static Task<string> GetStringAsync
            (this HttpClient httpClient, string url, StringPairCollection pairs,
            CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, pairs, cancellationToken);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        // POST METHODS
        public static async Task<HttpResponseMessage> PostAsync
            (this HttpClient httpClient, string url, StringPairCollection pairs,
            CancellationToken cancellationToken = default, string mediaType = "application/x-www-form-urlencoded")
        {
            return await httpClient.PostAsync(url,
                new StringContent(pairs.ToHttpQueryString(), Encoding.UTF8, mediaType),
                cancellationToken).ConfigureAwait(false);
        }

        public static async Task<string> PostToStringAsync
            (this HttpClient httpClient, string url, StringPairCollection pairs,
            CancellationToken cancellationToken = default, string mediaType = "application/x-www-form-urlencoded")
        {
            var response = await httpClient.PostAsync(url, pairs, cancellationToken, mediaType);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task<string> PostMultipartToStringAsync
            (this HttpClient httpClient, string url, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

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

        public static async Task<Bitmap> DownloadBitmapAsync
            (this HttpClient httpClient, string url, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            var imageStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            return new Bitmap(imageStream);
        }
    }

    public static class StringExtensions
    {
        public static T Deserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string SerializeCamelCase<T>(this T obj)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static string SerializeLowerCase<T>(this T obj)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new LowercasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(obj, settings);
        }
    }

    public static class BoolExtensions
    {
        public static int ToInt(this bool boolean)
        {
            return boolean ? 1 : 0;
        }
    }

    public static class CaptchaLanguageExtensions
    {
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
