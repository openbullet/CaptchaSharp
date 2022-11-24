using CaptchaSharp.Services.CapSolver.Requests.Tasks;

namespace CaptchaSharp.Services.CapSolver.Requests
{
    internal class CaptchaTaskRequest : Request
    {
        public CapSolverTaskProxyless Task { get; set; }
        public string AppId { get; set; } = "";
        public string LanguagePool { get; set; } = "en";
    }
}
