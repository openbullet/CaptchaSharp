using CaptchaSharp.Enums;
using System;

namespace CaptchaSharp.Models
{
    internal class CaptchaTask
    {
        public DateTime CreationDate { get; }
        public CaptchaType Type { get; set; }
        public long Id { get; }
        public bool Completed { get; set; } = false;

        public CaptchaTask(string id, CaptchaType type) : this(long.Parse(id), type) { }

        public CaptchaTask(long id, CaptchaType type)
        {
            Id = id;
            Type = type;
            CreationDate = DateTime.Now;
        }
    }
}
