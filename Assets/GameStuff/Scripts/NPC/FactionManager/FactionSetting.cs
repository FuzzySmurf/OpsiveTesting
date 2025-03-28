using System;
using UnityEngine;

namespace PolyGame.Faction
{
    [Serializable]
    public class FactionSetting
    {
        public EFaction FactionID { get { return _team; } }
        public EFaction LocalFriendlys { get { return _friendlys; } }

        [SerializeField] private EFaction _team;
        [SerializeField] private EFaction _friendlys;



        /// <summary>
        /// Changes FactionID with the given TeamID
        /// </summary>
        /// <param name="faction"></param>
        public void ChangeFactions(EFaction faction)
        {
            _team = faction;
        }

        /// <summary>
        /// Changed the given FactionID to be friendly
        /// </summary>
        /// <param name="faction"></param>
        public void AddFriendlyStatus(EFaction faction)
        {
            _friendlys |= faction;
        }

        /// <summary>
        /// Changes the given FactionID to be unfriendly
        /// </summary>
        /// <param name="faction"></param>
        public void RemoveFriendlyStaus(EFaction faction)
        {
            _friendlys &= ~faction;
        }

        /// <summary>
        /// Check if we are friendly with the given FactionID
        /// </summary>
        /// <param name="faction"></param>
        public bool CheckFriendlyStatus(EFaction faction)
        {
            bool localFriedly = ((_friendlys | _team) & faction) != 0;

            return true;
            //IFactionManager factionManager = GODependencyRegistrar.GetService<IFactionManager>();
            //return localFriedly | factionManager.CheckFriendlyStatus(FactionID, faction);
        }

        public bool HasFactions(EFaction factions)
        {
            if ((FactionID & factions) != 0)
                return true;

            return false;
        }
    }
}