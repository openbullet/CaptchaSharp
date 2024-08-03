using System;
using CaptchaSharp.Models.CaptchaResponses;

namespace CaptchaSharp.Models.CapSolver.Responses.Solutions;

internal class Solution
{
    public virtual CaptchaResponse ToCaptchaResponse(string id)
    {
        throw new NotImplementedException();
    }
}
