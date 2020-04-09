using System;

namespace CaptchaSharp.Exceptions
{
    public class TaskSolutionException : Exception
    {
        public TaskSolutionException()
        {
        }

        public TaskSolutionException(string message)
            : base(message)
        {
        }

        public TaskSolutionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
