using System;

namespace ARAWorks.Base.Enums
{
    [Flags]
    public enum EStatusEffectType
    {
        None = 0,
        Buff = 1,
        Debuff = 2,
        Heal = 4,
        Poison = 8,
        DoT = 16,
        Movement = 32,
    }
}
