
using ARAWorks.Base.Enums;
using ARAWorks.StatusEffectSystem.Statuses;
using System;
using System.Collections.Generic;

namespace ARAWorks.StatusEffectSystem.Handlers
{
    public interface IStatusEffectHandler
    {
        event Action<StatusEffectBase, bool> OnEffectActive;

        List<StatusEffectBase> GetStatusEffectList();
        List<StatusEffectBase> GetStatusEffectsByType(EStatusEffectType seType);

        bool HasStatusEffect(EStatusEffectType seType);
        bool HasStatusEffect(Type type);
        bool HasStatusEffect(StatusEffectBase effect);
        bool CanActivateEffect(Type type);
        void AddEffect(StatusEffectBase effect);
        void RemoveEffect(StatusEffectBase effect);
        void RemoveEffect(Type effectType);
        void RemoveAllEffects();
        void RemoveEffectsByType(EStatusEffectType seType);
    }
}