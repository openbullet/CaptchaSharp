using CaptchaSharp.Services.AntiCaptcha.Responses.Solutions;

namespace CaptchaSharp.Services.AntiCaptcha.Responses;

internal class GetTaskResultAntiCaptchaResponse : AntiCaptchaResponse
{
    public string Status { get; set; }
    public double Cost { get; set; }
    public AntiCaptchaTaskSolution AntiCaptchaTaskSolution { get; set; }
    public string Ip { get; set; }
    public double CreateTime { get; set; }
    public double EndTime { get; set; }
    public double? SolveCount { get; set; }

    public bool IsReady => Status != "processing";
}
