namespace CaptchaSharp.Services.CapSolver.Requests.Tasks.Proxied
{
    internal class RecaptchaV2EnterpriseTask : CapSolverTask
    {
        public string WebsiteURL { get; set; }
        public string WebsiteKey { get; set; }
        public string EnterprisePayload { get; set; }

        public RecaptchaV2EnterpriseTask()
        {
            Type = "RecaptchaV2EnterpriseTask";
        }
    }
}
