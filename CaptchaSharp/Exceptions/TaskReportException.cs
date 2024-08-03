using System;

namespace CaptchaSharp.Exceptions;

/// <summary>An exception that is thrown when a captcha failed to be reported as wrong.</summary>
public class TaskReportException : Exception
{
    /// <summary></summary>
    public TaskReportException()
    {
    }

    /// <summary></summary>
    public TaskReportException(string message)
        : base(message)
    {
    }

    /// <summary></summary>
    public TaskReportException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
