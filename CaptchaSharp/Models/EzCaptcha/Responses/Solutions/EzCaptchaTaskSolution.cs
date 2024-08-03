using System;
using CaptchaSharp.Models.CaptchaResponses;

namespace CaptchaSharp.Models.EzCaptcha.Responses.Solutions;

internal class EzCaptchaTaskSolution
{
    public virtual CaptchaResponse ToCaptchaResponse(string id)
    {
        throw new NotImplementedException();
    }
}
