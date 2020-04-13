using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace CaptchaSharp.Models
{
    public class StringPairCollection
    {
        private List<(string, string)> pairs = new List<(string, string)>();

        public StringPairCollection Add(string first, string second, bool addCondition = true)
        {
            if (addCondition)
                pairs.Add((first, second));

            return this;
        }

        public StringPairCollection Add<A,B>(A first, B second, bool addCondition = true)
        {
            return Add(first.ToString(), second.ToString(), addCondition);
        }

        public StringPairCollection Add(IEnumerable<(string, string)> pairsToAdd)
        {
            pairs = pairs.Concat(pairsToAdd).ToList();
            return this;
        }

        public string ToHttpQueryString()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            pairs.ForEach(p => query.Add(p.Item1, p.Item2));
            return query.ToString();
        }

        public MultipartFormDataContent ToMultipartFormDataContent()
        {
            var content = new MultipartFormDataContent();
            pairs.ForEach(p => content.Add(new StringContent(p.Item2), p.Item1));
            return content;
        }
    }
}
