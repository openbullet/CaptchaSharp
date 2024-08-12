namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;

internal class RecaptchaV2Task : CapSolverTask
{
    public string? WebsiteURL { get; set; }
    public string? WebsiteKey { get; set; }
    public bool IsInvisible { get; set; }

    public RecaptchaV2Task()
    {
        Type = "RecaptchaV2Task";
    }
}
