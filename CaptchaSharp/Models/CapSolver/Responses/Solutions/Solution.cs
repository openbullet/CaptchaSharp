using CaptchaSharp.Models;
using System;

namespace CaptchaSharp.Models.CapSolver.Responses.Solutions;

internal class Solution
{
    public virtual CaptchaResponse ToCaptchaResponse(string id)
    {
        throw new NotImplementedException();
    }
}