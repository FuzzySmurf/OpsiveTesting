using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    /// <summary>
    /// if canMoveWhileAttacking = false, you can set how long it will take until you can move again with moveAfterXTime.
    /// Otherwise, if canMoveWhileAttacking - false, and moveAfterXTime >= 0.0f,
    /// the defaultCharacterItem USE Event Complete will be used.
    /// </summary>
    public class SVMoveWhileAttacking
    {
        [Tooltip("Should the NPC be able to move while performing the attack ability?")]
        public SharedVariable<bool> canMoveWhileAttacking = true;
        [Tooltip("How long do we wait until we can move again? If 0.0f, will use the CharacterItem Use Event Complete event.")]
        public SharedVariable<float> moveAfterXTime = 0.0f;
    }
}