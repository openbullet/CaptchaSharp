using System;

namespace CaptchaSharp.Exceptions
{
    public class TaskCreationException : Exception
    {
        public TaskCreationException()
        {
        }

        public TaskCreationException(string message)
            : base(message)
        {
        }

        public TaskCreationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
