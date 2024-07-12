using CaptchaSharp.Enums;
using System;

namespace CaptchaSharp.Models;

/// <summary>Information about a captcha task</summary>
public class CaptchaTask
{
    /// <summary>When the task was created</summary>
    public DateTime CreationDate { get; }

    /// <summary>The type of captcha that is being solved</summary>
    public CaptchaType Type { get; set; }

    /// <summary>The id of the task</summary>
    public long Id { get; }

    /// <summary>The id of the task as a string</summary>
    public string IdString { get; }

    /// <summary>Whether the task is completed</summary>
    public bool Completed { get; set; } = false;

    /// <summary>Creates a <see cref="CaptchaTask"/> from a string id</summary>
    public CaptchaTask(string id, CaptchaType type)
    {
        IdString = id;

        if (long.TryParse(id, out long parsed))
        {
            Id = parsed;
        }

        Type = type;
        CreationDate = DateTime.Now;
    }

    /// <summary>Creates a <see cref="CaptchaTask"/> from a long id</summary>
    public CaptchaTask(long id, CaptchaType type)
    {
        Id = id;
        IdString = id.ToString();
        Type = type;
        CreationDate = DateTime.Now;
    }
}
