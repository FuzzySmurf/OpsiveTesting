using System;

namespace ARAWorks.Base.Enums
{
    /*  These are ordered in way to easlily tell if a type is of weapon, armor, or accesory.
        This is done by keep them in their own range.
        Note: Not all entries within a range need to be filled.
        Armor:      [00 - 09]
        Accesory:   [10 - 19]
        Weapon:     [20 - 29]

    */
    public enum EEquipmentSlotType
    {
        Helm = 0,
        Chest = 1,
        Shoulder = 2,
        Bracers = 3,
        Pants = 4,
        Boots = 5,
        Necklace = 10,
        Ring1 = 11,
        Ring2 = 12,
        MainHandWeapon = 20,
        OffHandWeapon = 21
    }
}