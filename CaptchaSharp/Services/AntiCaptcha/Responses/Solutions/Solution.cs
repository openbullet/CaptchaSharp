using CaptchaSharp.Models;
using System;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions
{
    internal class Solution
    {
        public virtual CaptchaResponse ToCaptchaResponse(long id)
        {
            throw new NotImplementedException();
        }
    }
}
