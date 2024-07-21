using CaptchaSharp.Models;
using System;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions;

internal class AntiCaptchaTaskSolution
{
    public virtual CaptchaResponse ToCaptchaResponse(string id)
    {
        throw new NotImplementedException();
    }
}
