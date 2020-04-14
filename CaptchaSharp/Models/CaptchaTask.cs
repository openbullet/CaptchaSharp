using CaptchaSharp.Enums;
using System;

namespace CaptchaSharp.Models
{
    /// <summary></summary>
    public class CaptchaTask
    {
        /// <summary></summary>
        public DateTime CreationDate { get; }

        /// <summary></summary>
        public CaptchaType Type { get; set; }

        /// <summary></summary>
        public long Id { get; }

        /// <summary></summary>
        public bool Completed { get; set; } = false;

        /// <summary></summary>
        public CaptchaTask(string id, CaptchaType type) : this(long.Parse(id), type) { }

        /// <summary></summary>
        public CaptchaTask(long id, CaptchaType type)
        {
            Id = id;
            Type = type;
            CreationDate = DateTime.Now;
        }
    }
}
