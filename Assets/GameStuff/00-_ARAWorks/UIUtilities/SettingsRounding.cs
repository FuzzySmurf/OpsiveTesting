using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.UIUtilities
{
    public static class SettingsRounding
    {
        public static bool HUDAdvanced { get; set; }
        public static bool TooltipAdvanced { get; set; }
        public static bool FloatingNumbersAdvanced { get; set; }


        public static string RoundValue(float input, ESettingsRoundingType roundingType)
        {

            string GetInt()
            {
                return Mathf.RoundToInt(input).ToString();
            }

            string GetFloat()
            {
                return Math.Round(input, 2).ToString();
            }

            string result = roundingType switch
            {
                ESettingsRoundingType.HUD when HUDAdvanced == false => GetInt(),
                ESettingsRoundingType.HUD when HUDAdvanced == true => GetFloat(),

                ESettingsRoundingType.Tooltip when TooltipAdvanced == false => GetInt(),
                ESettingsRoundingType.Tooltip when TooltipAdvanced == true => GetFloat(),

                ESettingsRoundingType.FloatingNumbers when FloatingNumbersAdvanced == false => GetInt(),
                ESettingsRoundingType.FloatingNumbers when FloatingNumbersAdvanced == true => GetFloat(),

                _ => ""
            };

            return result;
        }
    }
}