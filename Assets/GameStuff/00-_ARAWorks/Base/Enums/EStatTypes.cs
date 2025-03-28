using System;

namespace ARAWorks.Base.Enums
{
    public enum EStatTypes
    {
        //Core                  = [0 - 100]
        //Elemental Damage      = [101 - 200] 
        //Elemental Resistance  = [201 - 300]
        //Utility               = [301+]
        ArmorRating = 0,
        PhysicalDamage = 1,
        PhysicalResistance = 2,
        CriticalDamage = 3,
        CriticalChance = 4,
        MaxHealth = 5,
        MoveSpeed = 6,
        AttackSpeed = 7,
        HealthRegeneration = 8,
        HealingPower = 9,
        ArcaneDamage = 101,
        VoidDamage = 102,
        LightDamage = 103,
        FireDamage = 104,
        WaterDamage = 105,
        EarthDamage = 106,
        WindDamage = 107,
        PoisonDamage = 108,
        ArcaneResistance = 201,
        VoidResistance = 202,
        LightResistance = 203,
        FireResistance = 204,
        WaterResistance = 205,
        EarthResistance = 206,
        WindResistance = 207,
        PoisonResistance = 208,

        CastSpeed = 302,
        LifeLeech = 303,
        DamageReduction = 304
    }
}
