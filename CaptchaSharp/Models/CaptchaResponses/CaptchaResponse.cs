using System;

namespace CaptchaSharp.Models.CaptchaResponses;

/// <summary>A generic captcha response.</summary>
public class CaptchaResponse
{
    /// <summary>The captcha id which is needed to report the solution as bad.</summary>
    public required string Id { get; set; }

    /// <summary>The time when the solution was received.</summary>
    public DateTime CompletedAt { get; set; } = DateTime.Now;
}
