namespace CaptchaSharp.Models.CapSolver.Requests.Tasks;

internal class MtCaptchaTaskProxyless : CapSolverTaskProxyless
{
    public string? WebsiteKey { get; set; }
    public string? WebsiteURL { get; set; }

    public MtCaptchaTaskProxyless()
    {
        Type = "MtCaptchaTaskProxyLess";
    }
}
