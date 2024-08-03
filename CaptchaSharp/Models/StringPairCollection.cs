using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using CaptchaSharp.Enums;

namespace CaptchaSharp.Models;

/// <summary>
/// A collection of string pairs.
/// </summary>
public class StringPairCollection
{
    private List<(string, string)> _pairs = [];
    private StringPairValueHandling ValueHandling { get; }

    /// <summary></summary>
    public StringPairCollection(StringPairValueHandling valueHandling = StringPairValueHandling.AddIfNotNullOrEmpty)
    {
        ValueHandling = valueHandling;
    }
    
    /// <summary>
    /// Adds a new pair to the collection. The <paramref name="second"/> value is converted
    /// to string and handled according to the <see cref="ValueHandling"/> property.
    /// </summary>
    public StringPairCollection Add<TValue>(string first, TValue second)
    {
        var secondString = second?.ToString();
        
        switch (ValueHandling)
        {
            case StringPairValueHandling.AddIfNotNullOrEmpty when string.IsNullOrEmpty(secondString):
                return this;
            
            case StringPairValueHandling.AddIfNotNull when secondString is null:
                return this;
            
            default:
                _pairs.Add((first, secondString ?? string.Empty));

                return this;
        }
    }
    
    /// <summary>
    /// Adds a new pair to the collection only if the <paramref name="condition"/> is true.
    /// The <paramref name="second"/> value is converted
    /// to string and handled according to the <see cref="ValueHandling"/> property.
    /// </summary>
    public StringPairCollection Add<TValue>(string first, TValue? second, bool condition)
    {
        return condition ? Add(first, second) : this;
    }

    /// <summary>
    /// Adds multiple new pairs to the collection.
    /// </summary>
    public StringPairCollection Add(IEnumerable<(string, string)> pairsToAdd)
    {
        _pairs = _pairs.Concat(pairsToAdd).ToList();
        return this;
    }

    /// <summary>
    /// Outputs a string like <c>name1=value1&amp;name2=value2</c>.
    /// </summary>
    public string ToHttpQueryString()
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        _pairs.ForEach(p => query.Add(p.Item1, p.Item2));
        return query.ToString()!;
    }

    /// <summary>
    /// Outputs a new <see cref="MultipartFormDataContent"/> where each pair 
    /// of the collection becomes a <see cref="StringContent"/>.
    /// </summary>
    public MultipartFormDataContent ToMultipartFormDataContent()
    {
        var content = new MultipartFormDataContent();
        _pairs.ForEach(p => content.Add(new StringContent(p.Item2), p.Item1));
        return content;
    }
}
