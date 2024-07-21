namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied;

internal class FunCaptchaTask : CapSolverTask
{
    public string? WebsiteURL { get; set; }
    public string? WebsitePublicKey { get; set; }
    public string? FuncaptchaApiJSSubdomain { get; set; }

    public FunCaptchaTask()
    {
        Type = "FunCaptchaTask";
    }
}
