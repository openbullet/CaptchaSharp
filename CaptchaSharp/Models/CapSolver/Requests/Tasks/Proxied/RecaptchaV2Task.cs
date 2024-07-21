namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;

internal class RecaptchaV2Task : CapSolverTask
{
    public string WebsiteURL { get; set; }
    public string WebsiteKey { get; set; }
    public bool IsInvisible { get; set; }
    public string RecaptchaDataSValue { get; set; } = string.Empty;

    public RecaptchaV2Task()
    {
        Type = "RecaptchaV2Task";
    }
}