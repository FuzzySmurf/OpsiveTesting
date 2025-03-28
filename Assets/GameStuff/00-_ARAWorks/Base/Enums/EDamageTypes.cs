using System;

namespace ARAWorks.Base.Enums
{
    [Flags]
    public enum EDamageType
    {
        Null = 0,
        Pure = 1,
        Physical = 2,
        Healing = 4,
        Arcane = 8,
        Void = 16,
        Light = 32,
        Fire = 64,
        Water = 128,
        Earth = 256,
        Wind = 512,
        Poison = 1024,
        Critical = 2048
    }
}
