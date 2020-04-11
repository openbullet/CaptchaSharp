using CaptchaSharp.Enums;
using System;

namespace CaptchaSharp.Models
{
    internal class CaptchaTask
    {
        public DateTime CreationDate { get; }
        public CaptchaType Type { get; set; }
        public int Id { get; }
        public bool Completed { get; set; } = false;

        public CaptchaTask(string id, CaptchaType type) : this(int.Parse(id), type) { }

        public CaptchaTask(int id, CaptchaType type)
        {
            Id = id;
            Type = type;
            CreationDate = DateTime.Now;
        }
    }
}
