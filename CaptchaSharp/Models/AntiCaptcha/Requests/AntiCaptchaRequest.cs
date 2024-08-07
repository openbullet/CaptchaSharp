﻿namespace CaptchaSharp.Models.AntiCaptcha.Requests;

/// <summary>
/// Represents a request to the AntiCaptcha API.
/// </summary>
public class AntiCaptchaRequest
{
    /// <summary>
    /// Your AntiCaptcha API key.
    /// </summary>
    public string ClientKey { get; set; } = "";
}
