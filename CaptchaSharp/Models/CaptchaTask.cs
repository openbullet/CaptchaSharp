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
    public string Id { get; }

    /// <summary>Whether the task is completed</summary>
    public bool Completed { get; set; }

    /// <summary>Creates a <see cref="CaptchaTask"/> from a string id</summary>
    public CaptchaTask(string id, CaptchaType type)
    {
        Id = id;
        Type = type;
        CreationDate = DateTime.Now;
    }
}
