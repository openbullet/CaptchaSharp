using CaptchaSharp.Models.EzCaptcha.Responses.Solutions;

namespace CaptchaSharp.Models.EzCaptcha.Responses;

internal class GetTaskResultEzCaptchaResponse : EzCaptchaResponse
{
    public string? Status { get; set; }
    
    public EzCaptchaTaskSolution? EzCaptchaTaskSolution { get; set; }
    
    public bool IsReady => Status != "processing";
}
