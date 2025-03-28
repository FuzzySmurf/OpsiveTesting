using System;

namespace ARAWorks.Base.Contracts
{
    public class ContractStatsCore
    {
        public float ArmorRating { get; set; }
        public float PhysicalDamage { get; set; }
        public float ArcaneDamage { get; set; }
        public float PhysicalResistance { get; set; }
        public float ArcaneResistance { get; set; }
        public float CriticalDamage { get; set; }
        public float CriticalChance { get; set; }
        public float MaxHealth { get; set; }
        public float MoveSpeed { get; set; }
        public float AttackSpeed { get; set; }
        public float HealthRegeneration { get; set; }
        public float HealingPower { get; set; }


        public override string ToString()
        {
            return
            $@"--ContractStatsCore--
            ArmorRating: {ArmorRating}
            PhysicalDamage: {PhysicalDamage}
            ArcaneDamage: {ArcaneDamage}
            PhysicalResistance: {PhysicalResistance}
            ArcaneResistance: {ArcaneResistance}
            CriticalDamage: {CriticalDamage}
            CriticalChance: {CriticalChance}
            MaxHealth: {MaxHealth}
            MoveSpeed: {MoveSpeed}
            AttackSpeed: {AttackSpeed}
            HealthRegeneration: {HealthRegeneration}
            HealingPower: {HealingPower}";
        }

    }
}