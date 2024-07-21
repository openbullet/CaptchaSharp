namespace CaptchaSharp.Models.CapSolver.Requests.Tasks;

internal class FunCaptchaTaskProxyless : CapSolverTaskProxyless
{
    public string? WebsiteURL { get; set; }
    public string? WebsitePublicKey { get; set; }
    public string? FuncaptchaApiJSSubdomain { get; set; }

    public FunCaptchaTaskProxyless()
    {
        Type = "FunCaptchaTaskProxyless";
    }
}
