using System;
using System.Collections.Generic;
using System.Text;

namespace CaptchaSharp.Enums
{
    [Flags]
    public enum CaptchaServiceCapabilities
    {
        None = 0,
        LanguageGroup = 1,
        Language = 2,
        Phrases = 4,
        CaseSensitivity = 8,
        CharacterSets = 16,
        Calculations = 32,
        MinLength = 64,
        MaxLength = 128,
        Instructions = 256
    }
}
