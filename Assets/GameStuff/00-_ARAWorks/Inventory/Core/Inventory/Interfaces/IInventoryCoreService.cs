
namespace ARAWorks.Inventory
{
    /* NOTE: Use this for all exterior interactions that need access to the Inventory. 
     * Such as adding items to the inventory, or quest rewards, etc..
     * */
    public interface IInventoryCoreService : IInventoryCoreHandler, IInventoryDataHandler, IEquipmentDataHandler { }
}