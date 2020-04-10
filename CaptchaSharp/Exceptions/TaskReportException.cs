using System;
using System.Collections.Generic;
using System.Text;

namespace CaptchaSharp.Exceptions
{
    public class TaskReportException : Exception
    {
        public TaskReportException()
        {
        }

        public TaskReportException(string message)
            : base(message)
        {
        }

        public TaskReportException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
