using ARAWorks.Base.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARAWorks.Base.Contracts;
using ARAWorks.Damage.UIToolkit;

namespace ARAWorks.Damage
{
    public static class AdditivesMapper
    {
        public static string GetName(ContractDamageValues damage)
        {
            string name = "";

            if (damage.ModifierType == EDamageType.Pure)
            {
                name = "Pure ";
            }

            name += damage.DamageType switch
            {
                EDamageType.Arcane => "Arcane",
                EDamageType.Healing => "Healing",
                EDamageType.Physical => "Physical",
                EDamageType.Poison => "Poison",
                EDamageType.Light => "Light",
                EDamageType.Void => "Void",
                EDamageType.Fire => "Fire",
                EDamageType.Water => "Water",
                EDamageType.Earth => "Earth",
                EDamageType.Wind => "Wind",
                EDamageType.Fire | EDamageType.Wind => "Lightning",
                _ => "Null"
            };

            return name;
        }

        public static string GetTextClass(ContractDamageValues damage) => damage.DamageType switch
        {
            EDamageType.Arcane => UIConstantDamage.DamageNumbers.ColorArcane,
            EDamageType.Healing => UIConstantDamage.DamageNumbers.ColorHealing,
            EDamageType.Physical => UIConstantDamage.DamageNumbers.ColorPhysical,
            EDamageType.Poison => UIConstantDamage.DamageNumbers.ColorPoison,
            EDamageType.Light => UIConstantDamage.DamageNumbers.ColorLight,
            EDamageType.Void => UIConstantDamage.DamageNumbers.ColorVoid,
            EDamageType.Fire => UIConstantDamage.DamageNumbers.ColorFire,
            EDamageType.Water => UIConstantDamage.DamageNumbers.ColorWater,
            EDamageType.Earth => UIConstantDamage.DamageNumbers.ColorEarth,
            EDamageType.Wind => UIConstantDamage.DamageNumbers.ColorWind,
            (EDamageType.Fire | EDamageType.Wind) => UIConstantDamage.DamageNumbers.ColorLightning,
            _ => "Null"
        };

        public static string GetOutlineClass(ContractDamageValues damage) => damage.ModifierType switch
        {
            EDamageType.Pure => UIConstantDamage.DamageNumbers.ColorOutlinePure,
            EDamageType.Critical => UIConstantDamage.DamageNumbers.ColorOutlineCritical,
            _ => UIConstantDamage.DamageNumbers.ColorOutlineDefault
        };

    }
}
