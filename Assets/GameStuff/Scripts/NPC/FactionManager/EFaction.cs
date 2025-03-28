using System;

namespace PolyGame.Faction
{
    //Note Max Factions is 32
    [Flags]
    public enum EFaction
    {
        None = 0,                           // 0b_0000_0000
        Player = 1,                         // 0b_0000_0001
        NPC = 2,                            // 0b_0000_0010
        Enemy = 4,                          // 0b_0000_0100
        Object = 8,                         // 0b_0000_1000
        All = Player | NPC | Enemy          // 0b_1111_1111
    }
}