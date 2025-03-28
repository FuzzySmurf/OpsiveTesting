
namespace PolyGame.Faction
{
    public interface IFactionManager
    {
        /// <summary>
        /// Check if our Faction is friendly with their Faction
        /// </summary>
        bool CheckFriendlyStatus(EFaction ourFaction, EFaction theirFaction);

        /// <summary>
        /// Get a Faction flag with bits set towards friendlies.
        /// </summary>
        /// <param name="ourFaction">Faction we are gathering relations for.</param>
        /// <returns>Returns a Faction flag with bits set to 1 if they correspond to a friendly based on relation.</returns>
        EFaction GetFriendlies(EFaction ourFaction);

        /// <summary>
        /// Get a Faction flag with bits set towards enemies.
        /// </summary>
        /// <param name="ourFaction">Faction we are gathering relations for.</param>
        /// <returns>Returns a Faction flag with bits set to 1 if they correspond to a enemy based on relation.</returns>
        EFaction GetEnemies(EFaction ourFaction);

        /// <summary>
        /// Makes our Faction and their Faction friendly
        /// </summary>
        /// <param name="faction"></param>
        void AddFriendlyStatus(EFaction ourFaction, EFaction theirFaction);

        /// <summary>
        /// Makes our Faction and their Faction hostile 
        /// </summary>
        void RemoveFriendlyStatus(EFaction ourFaction, EFaction theirFaction);
    }
}