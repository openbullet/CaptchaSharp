namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;

internal class RecaptchaV3Task : CapSolverTask
{
    public string WebsiteURL { get; set; }
    public string WebsiteKey { get; set; }
    public string PageAction { get; set; }
    public float MinScore { get; set; }
    public bool IsEnterprise { get; set; }

    public RecaptchaV3Task()
    {
        Type = "RecaptchaV3Task";
    }
}