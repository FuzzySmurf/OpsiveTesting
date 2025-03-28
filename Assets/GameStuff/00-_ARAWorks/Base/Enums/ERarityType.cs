using System;
using System.Collections.Generic;
using System.Linq;

namespace ARAWorks.Base.Enums
{
    //public enum ERarityType
    //{
    //    Common = 0,
    //    Uncommon = 1,
    //    Rare = 2,
    //    Epic = 3,
    //    Legendary = 4
    //}

    public partial class ERarityType : Enumeration
    {
        public static readonly ERarityType Common = new ERarityType(0, "Common");
        public static readonly ERarityType Uncommon = new ERarityType(1, "Uncommon");
        public static readonly ERarityType Rare = new ERarityType(2, "Rare");
        public static readonly ERarityType Epic = new ERarityType(3, "Epic");
        public static readonly ERarityType Legendary = new ERarityType(4, "Legendary");

        public ERarityType() { }

        public ERarityType(int value, string displayName) : base(value, displayName) { }

        public static ERarityType CheckForDefault(ERarityType val)
        {
            if(val == null) 
                return ERarityType.Common;
            return val;
        }

        public static ERarityType FromName(string name)
        {
            return ERarityType.FromDisplayName<ERarityType>(name);
        }

        public static ERarityType IncrementByOne(ERarityType rarityType)
        {
            int val = rarityType.Value + 1;
            List<ERarityType> all = ERarityType.GetAll<ERarityType>().ToList();
            ERarityType ret = all.FirstOrDefault(x => x.Value == val);

            return (ret == null) ? ERarityType.Legendary : ret;
        }
    }
}
