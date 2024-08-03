using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CaptchaSharp.Extensions;

/// <summary>Extensions for a <see cref="string"/>.</summary>
public static class StringExtensions
{
    /// <summary>Deserializes a json string to a given type.</summary>
    public static T Deserialize<T>(this string json) where T : notnull
    {
        return JsonConvert.DeserializeObject<T>(json)
            ?? throw new JsonSerializationException("Failed to deserialize json string.");
    }
    
    /// <summary>Serializes an object to a json string.</summary>
    public static string Serialize<T>(this T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    /// <summary>Serializes an object to a json string and converts the property names 
    /// to a camelCase based convention.</summary>
    public static string SerializeCamelCase<T>(this T obj)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        return JsonConvert.SerializeObject(obj, settings);
    }
}
