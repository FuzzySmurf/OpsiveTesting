using ARAWorks.Base.Enums;
using System;

namespace ARAWorks.Base.Contracts
{
    public class ContractAttributes
    {
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }

        public int this[EAttributeTypes attribute]
        {
            get
            {
                int val = 0;
                switch (attribute)
                {
                    case EAttributeTypes.Strength:
                        val = Strength;
                        break;
                    case EAttributeTypes.Wisdom:
                        val = Wisdom;
                        break;
                    case EAttributeTypes.Intelligence:
                        val = Intelligence;
                        break;
                    case EAttributeTypes.Constitution:
                        val = Constitution;
                        break;
                    case EAttributeTypes.Agility:
                        val = Agility;
                        break;
                }

                return val;
            }
        }

        public override string ToString()
        {
            return
            $@"--ContractAttributes--
            Strength: {Strength}
            Agility: {Agility}
            Constitution: {Constitution}
            Intelligence: {Intelligence}
            Wisdom: {Wisdom}";
        }
    }
}