using CaptchaSharp.Models;

namespace CaptchaSharp.Services.AntiCaptcha.Responses.Solutions
{
    internal interface ITaskSolution
    {
        CaptchaResponse ToCaptchaResponse(int id);
    }
}
