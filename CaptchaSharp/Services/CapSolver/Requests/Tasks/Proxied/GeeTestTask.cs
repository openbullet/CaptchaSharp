namespace CaptchaSharp.Services.CapSolver.Requests.Tasks.Proxied
{
    internal class GeeTestTask : CapSolverTask
    {
        public string WebsiteURL { get; set; }
        public string Gt { get; set; }
        public string Challenge { get; set; }
        public string? GeetestApiServerSubdomain { get; set; }

        public GeeTestTask()
        {
            Type = "GeeTestTask";
        }
    }
}
