namespace CaptchaSharp.Models.CapSolver.Requests.Tasks.Proxied
{
    internal class HCaptchaTask : CapSolverTask
    {
        public string WebsiteKey { get; set; }
        public string WebsiteURL { get; set; }

        public HCaptchaTask()
        {
            Type = "HCaptchaTask";
        }
    }
}
