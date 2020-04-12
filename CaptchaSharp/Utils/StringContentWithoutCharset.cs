using System.Net.Http;
using System.Text;

namespace CaptchaSharp.Utils
{
    public class StringContentWithoutCharset : StringContent
    {
        public StringContentWithoutCharset(string content) : base(content)
        {
        }

        public StringContentWithoutCharset(string content, Encoding encoding) : base(content, encoding)
        {
            Headers.ContentType.CharSet = "";
        }

        public StringContentWithoutCharset(string content, Encoding encoding, string mediaType) : base(content, encoding, mediaType)
        {
            Headers.ContentType.CharSet = "";
        }

        public StringContentWithoutCharset(string content, string mediaType) : base(content, Encoding.UTF8, mediaType)
        {
            Headers.ContentType.CharSet = "";
        }
    }
}
