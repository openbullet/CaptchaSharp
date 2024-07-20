namespace CaptchaSharp.Services.CapSolver.Requests;

internal class GetTaskResultRequest : Request
{
    public required string TaskId { get; set; }
}
