﻿namespace CaptchaSharp.Models.AntiCaptcha.Requests.Tasks;

internal class RecaptchaV3TaskProxyless : AntiCaptchaTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    public string? PageAction { get; set; }
    public float MinScore { get; set; }
    public bool IsEnterprise { get; set; }

    public RecaptchaV3TaskProxyless()
    {
        Type = "RecaptchaV3TaskProxyless";
    }
}
