using System;

namespace CaptchaSharp.Enums;

/// <summary></summary>
[Flags]
public enum CaptchaType
{
    /// <summary>A captcha that is a plaintext question.</summary>
    TextCaptcha = 1 << 0,

    /// <summary>A captcha that is made by an image with some text to recognize.</summary>
    ImageCaptcha = 1 << 1,

    /// <summary>A type of puzzle captcha.</summary>
    FunCaptcha = 1 << 2,

    /// <summary>The Google ReCaptcha v2.</summary>
    ReCaptchaV2 = 1 << 3,

    /// <summary>The Google ReCaptcha v3.</summary>
    ReCaptchaV3 = 1 << 4,

    /// <summary>A type of token captcha.</summary>
    HCaptcha = 1 << 5,

    /// <summary>A type of token captcha.</summary>
    KeyCaptcha = 1 << 6,

    /// <summary>A type of challenge based captcha.</summary>
    GeeTest = 1 << 7,

    /// <summary>A type of token captcha.</summary>
    Capy = 1 << 8,

    /// <summary>A type of challenge based captcha.</summary>
    DataDome = 1 << 9,
    
    /// <summary>Cloudflare's Turnstile captcha.</summary>
    CloudflareTurnstile = 1 << 10,
    
    /// <summary>Lemin Cropped captcha.</summary>
    LeminCropped = 1 << 11,
    
    /// <summary>Amazon WAF captcha.</summary>
    AmazonWaf = 1 << 12,
    
    /// <summary>Cyber SiARA captcha.</summary>
    CyberSiAra = 1 << 13,
    
    /// <summary>MT Captcha.</summary>
    MtCaptcha = 1 << 14,
    
    /// <summary>Cut Captcha.</summary>
    CutCaptcha = 1 << 15,
}
