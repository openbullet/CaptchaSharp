namespace CaptchaSharp.Services.CapSolver.Requests.Tasks.Proxied
{
    internal class DataDomeTask : CapSolverTask
    {
        public string WebsiteURL { get; set; }
        public string CaptchaURL { get; set; }

        public DataDomeTask()
        {
            Type = "DatadomeSliderTask";
        }
    }
}
