using System;

namespace CaptchaSharp.Enums;

/// <summary></summary>
[Flags]
public enum CaptchaServiceCapabilities
{
    /// <summary></summary>
    None = 0,

    /// <summary></summary>
    LanguageGroup = 1 << 0,

    /// <summary></summary>
    Language = 1 << 1,

    /// <summary></summary>
    Phrases = 1 << 2,

    /// <summary></summary>
    CaseSensitivity = 1 << 3,

    /// <summary></summary>
    CharacterSets = 1 << 4,

    /// <summary></summary>
    Calculations = 1 << 5,

    /// <summary></summary>
    MinLength = 1 << 6,

    /// <summary></summary>
    MaxLength = 1 << 7,

    /// <summary></summary>
    Instructions = 1 << 8,
}
