using System;

namespace CaptchaSharp.Models;

/// <summary>A generic captcha response.</summary>
public class CaptchaResponse
{
    // TODO: All captcha ids should be strings, then parse at need. Remove the long id.
    
    /// <summary>The captcha id which is needed to report the solution as bad.</summary>
    public long Id
    {
        get => long.Parse(IdString);
        init => IdString = value.ToString();
    }
    
    /// <summary>
    /// Whether the captcha id is a long int or a string.
    /// </summary>
    public bool IsLongId => long.TryParse(IdString, out _);

    /// <summary>
    /// The captcha id which is needed to report the solution, if it's
    /// a string instead of a long int.
    /// </summary>
    public string IdString { get; set; } = "0";

    /// <summary>The time when the solution was received.</summary>
    public DateTime Time { get; set; } = DateTime.Now;
}
