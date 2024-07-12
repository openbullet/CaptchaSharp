using System;

namespace CaptchaSharp.Enums
{
    /// <summary></summary>
    [Flags]
    public enum CaptchaType
    {
        /// <summary>A captcha that is a plaintext question.</summary>
        TextCaptcha,

        /// <summary>A captcha that is made by an image with some text to recognize.</summary>
        ImageCaptcha,

        /// <summary>A type of puzzle captcha.</summary>
        FunCaptcha,

        /// <summary>The Google ReCaptcha v2.</summary>
        ReCaptchaV2,

        /// <summary>The Google ReCaptcha v3.</summary>
        ReCaptchaV3,

        /// <summary>A type of token captcha.</summary>
        HCaptcha,

        /// <summary>A type of token captcha.</summary>
        KeyCaptcha,

        /// <summary>A type of challenge based captcha.</summary>
        GeeTest,

        /// <summary>A type of token captcha.</summary>
        Capy,

        /// <summary>A type of challenge based captcha.</summary>
        DataDome,

        /// <summary>The Cloudflare Turnstile captcha.</summary>
        Turnstile
    }
}
