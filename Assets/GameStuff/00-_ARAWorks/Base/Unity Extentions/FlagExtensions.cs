using ARAWorks.Base.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Base.Extensions
{

    public static class FlagExtentions
    {
        /// <summary>
        ///  Extension method to check if a layer is in a layermask
        /// </summary>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        /// <summary>
        /// Checks if the layer exists in the layerMasks.
        /// </summary>
        /// <returns>True if exists.</returns>
        public static bool DoesLayerExist(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        public static LayerMask Combine(this LayerMask mask1, LayerMask mask2)
        {
            return mask1 | mask2;
        }

        /// <summary>
        /// Converts a DamageType to its stat damage equivalent.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EStatTypes ToStatDamage(this EDamageType type)
        {
            if (type == EDamageType.Physical || type == EDamageType.Null || type == EDamageType.Pure || type == EDamageType.Critical || type.HasFlags())
            {
                return EStatTypes.PhysicalDamage;
            }

            return (EStatTypes)((int)type.FlagIndexToIntIndex() - 4 + 101);
        }

        /// <summary>
        /// Converts a DamageType to its stat resistance equivalent.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EStatTypes ToStatResistance(this EDamageType type)
        {
            if (type == EDamageType.Physical || type == EDamageType.Null || type == EDamageType.Pure || type == EDamageType.Critical || type.HasFlags())
            {
                return EStatTypes.PhysicalResistance;
            }

            return (EStatTypes)((int)type.FlagIndexToIntIndex() - 4 + 201);
        }

        /// <summary>
        /// Used to check if multiple flags have been set.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasFlags(this Enum type)
        {
            return Enum.IsDefined(type.GetType(), type) == false;
        }

        public static int FlagIndexToIntIndex(this Enum flag)
        {
            return Array.IndexOf(Enum.GetValues(flag.GetType()), flag);
        }

        public static List<EDamageType> GetFlags(this EDamageType type)
        {
            List<EDamageType> damages = new List<EDamageType>();

            //This should always be an adative 
            foreach (EDamageType value in EDamageType.GetValues(type.GetType()))
            {
                if (type.HasFlag(value) && value != 0)
                {
                    damages.Add(value);
                }
            }

            return damages;
        }

        public static bool HasAnyFlag<T>(this T value, T compareFlags) where T : Enum
        {
            int intValue = (int)(object)value;
            int intCompareFlags = (int)(object)compareFlags;
            return (intValue & intCompareFlags) != 0;
        }
    }
}