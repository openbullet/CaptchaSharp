using System;

namespace CaptchaSharp.Exceptions
{
    /// <summary>An exception that is thrown when a captcha could not be solved.</summary>
    public class TaskSolutionException : Exception
    {
        /// <summary></summary>
        public TaskSolutionException()
        {
        }

        /// <summary></summary>
        public TaskSolutionException(string message)
            : base(message)
        {
        }

        /// <summary></summary>
        public TaskSolutionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
