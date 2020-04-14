using System;

namespace CaptchaSharp.Enums
{
    [Flags]
    public enum CaptchaType
    {
        TextCaptcha,
        ImageCaptcha,
        FunCaptcha,
        ReCaptchaV2,
        ReCaptchaV3,
        HCaptcha,
        KeyCaptcha,
        GeeTest,
        Capy
    }
}
