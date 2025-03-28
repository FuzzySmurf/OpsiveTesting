using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace PolyGame.Faction
{
    public class FactionManager : SerializedMonoBehaviour, IFactionManager
    {
        [OdinSerialize, DictionaryDrawerSettings(KeyLabel = "Faction", ValueLabel = "Faction Relations")]
        private Dictionary<EFaction, EFaction> _factionRelations = new Dictionary<EFaction, EFaction>();

        public bool CheckFriendlyStatus(EFaction ourFaction, EFaction theirFaction)
        {
            return (_factionRelations[ourFaction] & theirFaction) != 0;
        }

        public EFaction GetFriendlies(EFaction ourFaction)
        {
            //Return our faction relations with our faction added.
            return _factionRelations[ourFaction] | ourFaction;
        }
        
        public EFaction GetEnemies(EFaction ourFaction)
        {
            return EFaction.All & ~(GetFriendlies(ourFaction));
        }

        private string ConvertToBits(EFaction choice)
        {
            return Convert.ToString((int)choice, 2).PadLeft(8, '0') + " | " + choice.ToString();
        }
                
        public void AddFriendlyStatus(EFaction ourFaction, EFaction theirFaction)
        {
            //Set ourFaction relation to friendly with theirFation
            _factionRelations[ourFaction] |= theirFaction;

            //Set theirFation relation to friendly with ourFaction
            _factionRelations[theirFaction] |= ourFaction;
        }

        public void RemoveFriendlyStatus(EFaction ourFaction, EFaction theirFaction)
        {
            //Set ourFaction relation to hostile with theirFation
            _factionRelations[ourFaction] &= ~theirFaction;

            //Set theirFaction relation to hostile with ourFaction
            _factionRelations[theirFaction] &= ~ourFaction;
        }
    }
}
