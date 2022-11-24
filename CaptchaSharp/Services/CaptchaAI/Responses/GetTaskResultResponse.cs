using CaptchaSharp.Services.CaptchaAI.Responses.Solutions;

namespace CaptchaSharp.Services.CaptchaAI.Responses
{
    internal class GetTaskResultResponse : Response
    {
        public string Status { get; set; }
        public double Cost { get; set; }
        public Solution Solution { get; set; }
        public string Ip { get; set; }
        public double CreateTime { get; set; }
        public double EndTime { get; set; }
        public double? SolveCount { get; set; }

        public bool IsReady => Status != "processing";
    }
}
