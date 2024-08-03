using System;

namespace CaptchaSharp.Exceptions;

/// <summary>An exception that is thrown when the credentials are invalid.</summary>
public class BadAuthenticationException : Exception
{
    /// <summary></summary>
    public BadAuthenticationException()
    {
    }

    /// <summary></summary>
    public BadAuthenticationException(string message)
        : base(message)
    {
    }

    /// <summary></summary>
    public BadAuthenticationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
