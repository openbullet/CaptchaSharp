using System;

namespace CaptchaSharp.Models.SolveCaptcha.Responses.Solutions;

internal class SolveCaptchaTaskSolution
{
    public virtual CaptchaResponse ToCaptchaResponse(string id)
    {
        throw new NotImplementedException();
    }
}
