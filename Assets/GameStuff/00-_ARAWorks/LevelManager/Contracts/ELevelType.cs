using ARAWorks.Base.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.LevelManager
{
    //public enum EnumLevelType
    //{
    //    None = 0,
    //    Main_Menu = 1,
    //    Loading_Screen = 2,
    //    Game_Level = 3,
    //    PlayerUI = 4
    //}

    public class ELevelType : Enumeration
    {
        public static readonly ELevelType None = new ELevelType(0, "None");
        public static readonly ELevelType MainMenu = new ELevelType(1, "Main Menu");
        public static readonly ELevelType LoadingScreen = new ELevelType(2, "Loading Screen");
        public static readonly ELevelType GameLevel = new ELevelType(3, "Game Level");
        public static readonly ELevelType PlayerUI = new ELevelType(4, "PlayerUI");
        public static readonly ELevelType PlayerCharacter = new ELevelType(5, "PlayerCharacter");

        public ELevelType() { }

        public ELevelType(int value, string displayName) : base(value, displayName) { }

        public static ELevelType CheckForDefault(ELevelType val)
        {
            if (val == null)
                return ELevelType.None;
            return val;
        }

        public static ELevelType FromName(string name)
        {
            return ELevelType.FromDisplayName<ELevelType>(name);
        }
    }
}
