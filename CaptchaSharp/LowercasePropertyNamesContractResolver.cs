namespace Newtonsoft.Json.Serialization
{
    public class LowercasePropertyNamesContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}
