using CaptchaSharp.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public static async Task<string> GetStringAsync
            (this HttpClient httpClient, string url, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task<string> PostMultipartAsync
            (this HttpClient httpClient, string url, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.PostAsync(url, content, cancellationToken).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public static async Task<string> PostJsonAsync<T>
            (this HttpClient httpClient, string url, T content, CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(content);
            var response = await httpClient.PostAsync(url, new StringContentWithoutCharset(json, "application/json"), cancellationToken);
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
    }

    public static class BoolExtensions
    {
        public static int ToInt(this bool boolean)
        {
            return boolean ? 1 : 0;
        }
    }

    // Fluent interface
    public static class MultipartExtensions
    {
        public static MultipartFormDataContent Add
            (this MultipartFormDataContent multipartFormDataContent, string name, string value, bool condition = true)
        {
            if (!condition)
                return multipartFormDataContent;

            multipartFormDataContent.Add(new StringContent(value), name);
            return multipartFormDataContent;
        }

        public static MultipartFormDataContent Add
            (this MultipartFormDataContent multipartFormDataContent, IEnumerable<(string, string)> stringContents, bool condition = true)
        {
            if (!condition)
                return multipartFormDataContent;

            stringContents.ToList()
                .ForEach(p => multipartFormDataContent.Add(new StringContent(p.Item2, Encoding.UTF8), p.Item1));

            return multipartFormDataContent;
        }

        public static MultipartFormDataContent Add
            (this MultipartFormDataContent multipartFormDataContent, IEnumerable<KeyValuePair<string, string>> stringContents, bool condition = true)
        {
            if (!condition)
                return multipartFormDataContent;

            stringContents.ToList()
                .ForEach(p => multipartFormDataContent.Add(new StringContent(p.Value, Encoding.UTF8), p.Key));

            return multipartFormDataContent;
        }
    }
}
