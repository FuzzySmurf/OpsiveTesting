using System;
using UnityEngine;

namespace ARAWorks.Base.Contracts
{
    public class ContractItem
    {
        public string GlobalItemIDRef { get; set; }

        public int Quantity { get; set; }

        public string Name { get; set; }
        public string ReferenceName { get; set; }

        /// <summary>
        /// The item description.
        /// </summary>
        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int ValueBuy { get; set; }

        public int ValueSell { get; set; }

        public bool IsUnavailable { get; set; }

        public bool IsQuestItem { get; set; }

        /// <summary>
        /// The reference path to the icon.
        /// </summary>
        public string ItemIconPath { get; set; }

        public virtual bool IsStackable
        {
            get
            {
                if (GetType() == typeof(ContractItemArmor) || GetType() == typeof(ContractItemWeapon))
                    return false;
                else
                    return true;
            }
        }
    }
}
