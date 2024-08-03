namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;

internal class MtCaptchaTask : CapSolverTask
{
    public string? WebsiteKey { get; set; }
    public string? WebsiteURL { get; set; }

    public MtCaptchaTask()
    {
        Type = "MtCaptchaTask";
    }
}
