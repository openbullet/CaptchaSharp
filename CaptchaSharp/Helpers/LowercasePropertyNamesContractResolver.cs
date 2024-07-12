using Newtonsoft.Json.Serialization;

namespace CaptchaSharp.Helpers;

internal class LowercasePropertyNamesContractResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        return propertyName.ToLower();
    }
}
