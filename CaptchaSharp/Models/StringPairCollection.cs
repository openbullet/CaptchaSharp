using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace CaptchaSharp.Models;

/// <summary>A collection of string pairs.</summary>
public class StringPairCollection
{
    private List<(string, string)> _pairs = [];

    /// <summary>Adds a new pair to the collection if <paramref name="addCondition"/> is true.</summary>
    public StringPairCollection Add(string first, string second, bool addCondition = true)
    {
        if (addCondition)
        {
            _pairs.Add((first, second));
        }

        return this;
    }

    /// <summary>Adds a new pair to the collection if <paramref name="addCondition"/> is true by 
    /// calling the ToString() method on <paramref name="first"/> and <paramref name="second"/>.</summary>
    public StringPairCollection Add<TKey,TValue>(TKey first, TValue second, bool addCondition = true)
        where TKey : notnull
        where TValue : notnull
    {
        return Add(first.ToString()!, second.ToString()!, addCondition);
    }

    /// <summary>Adds multiple new pairs to the collection.</summary>
    public StringPairCollection Add(IEnumerable<(string, string)> pairsToAdd)
    {
        _pairs = _pairs.Concat(pairsToAdd).ToList();
        return this;
    }

    /// <summary>Outputs a string like <c>name1=value1&amp;name2=value2</c></summary>
    public string ToHttpQueryString()
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        _pairs.ForEach(p => query.Add(p.Item1, p.Item2));
        return query.ToString()!;
    }

    /// <summary>Outputs a new <see cref="MultipartFormDataContent"/> where each pair 
    /// of the collection becomes a <see cref="StringContent"/>.</summary>
    public MultipartFormDataContent ToMultipartFormDataContent()
    {
        var content = new MultipartFormDataContent();
        _pairs.ForEach(p => content.Add(new StringContent(p.Item2), p.Item1));
        return content;
    }
}
