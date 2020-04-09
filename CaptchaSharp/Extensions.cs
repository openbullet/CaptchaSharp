using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
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
            MemoryStream stream = new MemoryStream();
            image.Save(stream, format);
            byte[] imageBytes = stream.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

        public static Bitmap ToBitmap(this string base64, ImageFormat format)
        {
            throw new NotImplementedException();
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
        public static async Task<T> GetJsonAsync<T>(this HttpClient httpClient, string url, CancellationToken cancellationToken = default)
        {
            return JsonConvert.DeserializeObject<T>(await GetStringAsync(httpClient, url, cancellationToken));
        }

        public static async Task<string> GetStringAsync(this HttpClient httpClient, string url, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
