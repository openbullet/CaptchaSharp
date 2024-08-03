namespace CaptchaSharp.Enums;

/// <summary>
/// Specifies how to handle the values of the string pairs.
/// </summary>
public enum StringPairValueHandling
{
    /// <summary>
    /// Always add the pair to the collection. Null values are converted to empty strings.
    /// </summary>
    AlwaysAdd,
    
    /// <summary>
    /// Add the pair to the collection only if the value is not null.
    /// </summary>
    AddIfNotNull,
    
    /// <summary>
    /// Add the pair to the collection only if the value is not null or empty.
    /// </summary>
    AddIfNotNullOrEmpty,
}
