using ARAWorks.Damage.Interfaces;
using PolyGame.Faction;

namespace PolyGame.Damage
{
    public interface ITargetExt : ITarget
    {
        FactionSetting Faction { get; }
    }
}