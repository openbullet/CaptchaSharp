using System;

namespace CaptchaSharp.Models
{
    public class CaptchaTask
    {
        public DateTime CreationDate { get; }
        public string Id { get; }
        public bool Completed { get; set; } = false;

        public CaptchaTask(string id)
        {
            Id = id;
            CreationDate = DateTime.Now;
        }
    }
}
