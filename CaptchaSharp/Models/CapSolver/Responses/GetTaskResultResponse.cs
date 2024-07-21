using CaptchaSharp.Models.CapSolver.Responses.Solutions;

namespace CaptchaSharp.Models.CapSolver.Responses;

internal class GetTaskResultResponse : Response
{
    public string? Status { get; set; }
    public double Cost { get; set; }
    public required Solution Solution { get; set; }
    public string? Ip { get; set; }
    public double CreateTime { get; set; }
    public double EndTime { get; set; }
    public double? SolveCount { get; set; }

    public bool IsReady => Status != "processing";
}
