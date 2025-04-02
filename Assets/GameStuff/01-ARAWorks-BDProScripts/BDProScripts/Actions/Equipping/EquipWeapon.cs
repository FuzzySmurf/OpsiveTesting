using ARAWorks.BehaviourDesignerPro;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.Shared.Game;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using Opsive.UltimateCharacterController.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyGame.BehaviourDesignerPro
{
    /* Notes:
     * * Used to equip a weapon by Name instead of slotID.
     * * This can be promoted to the ARAWorks library, However. This also uses a few 'custom' scripts that I made.
     * *    the custom scripts that I added were itemSetManager.GetItemByName(...). in the core opsive scripts.
     * */
    public class EquipWeapon : EnemyAction
    {
        public SharedVariable<int> weaponInventoryID = -1;
        public SharedVariable<string> weaponCharacterItem = "Body";
        protected EquipUnequip _equipUnequip;
        public SharedVariable<bool> isDebug = false;
        private bool _isEquipped = false;
        private ItemSetManager _itemSetManager;

        public override void OnAwake()
        {
            base.OnAwake();
            _equipUnequip = _characterLocomotion.GetAbility<EquipUnequip>();
            _itemSetManager = _equipUnequip.StateBoundGameObject.GetCachedComponent<ItemSetManager>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_equipUnequip != null)
            {
                if (weaponInventoryID.Value == -1)
                    weaponInventoryID.Value = GetItemByName(weaponCharacterItem.Value);

                _isEquipped = _equipUnequip.StartEquipUnequip(weaponInventoryID.Value);
            }

            if (isDebug.Value)
                Debug.Log($"Character {this.transform.parent.name} has triggerred EquipUnequip ({_isEquipped}).");

            return base.OnUpdate();
        }

        private int GetItemByName(string name)
        {
            var val = _itemSetManager.GetItemByName(_equipUnequip.ItemSetGroupIndex, name);
            return val;
        }
    }
}