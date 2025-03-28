using ARAWorks.StatusEffectSystem.Handlers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Damage.Interfaces
{
    public interface ITarget
    {
        IStatusEffectHandler GetStatusEffectHandler();
        GameObject GetGameObject();
        Vector3 GetPosition();
        Collider GetCollider();
    }
}