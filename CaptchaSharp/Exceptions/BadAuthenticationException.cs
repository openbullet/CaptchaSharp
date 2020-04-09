using System;

namespace CaptchaSharp.Exceptions
{
    public class BadAuthenticationException : Exception
    {
        public BadAuthenticationException()
        {
        }

        public BadAuthenticationException(string message)
            : base(message)
        {
        }

        public BadAuthenticationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
