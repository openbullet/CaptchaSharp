using System;
using System.Collections.Generic;
using System.Text;

namespace CaptchaSharp.Enums
{
    /// <summary></summary>
    [Flags]
    public enum CaptchaServiceCapabilities
    {
        /// <summary></summary>
        None = 0,

        /// <summary></summary>
        LanguageGroup = 1,

        /// <summary></summary>
        Language = 2,

        /// <summary></summary>
        Phrases = 4,

        /// <summary></summary>
        CaseSensitivity = 8,

        /// <summary></summary>
        CharacterSets = 16,

        /// <summary></summary>
        Calculations = 32,

        /// <summary></summary>
        MinLength = 64,

        /// <summary></summary>
        MaxLength = 128,

        /// <summary></summary>
        Instructions = 256
    }
}
