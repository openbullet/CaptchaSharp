namespace CaptchaSharp.Services.CapSolver.Requests.Tasks
{
    internal class HCaptchaTaskProxyless : CapSolverTaskProxyless
    {
        public string WebsiteKey { get; set; }
        public string WebsiteURL { get; set; }

        public HCaptchaTaskProxyless()
        {
            Type = "HCaptchaTaskProxyless";
        }
    }
}
