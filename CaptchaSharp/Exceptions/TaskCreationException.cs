using System;

namespace CaptchaSharp.Exceptions
{
    /// <summary>An exception that is thrown when a captcha task could not be created on the remote server.</summary>
    public class TaskCreationException : Exception
    {
        /// <summary></summary>
        public TaskCreationException()
        {
        }

        /// <summary></summary>
        public TaskCreationException(string message)
            : base(message)
        {
        }

        /// <summary></summary>
        public TaskCreationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
