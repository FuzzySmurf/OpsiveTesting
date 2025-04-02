using Opsive.Shared.Inventory;
using Opsive.UltimateCharacterController.Inventory;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ItemSetManagerExt
{
    /// <summary>
    /// Returns the Item Set by Name.
    /// </summary>
    /// <param name="groupIndex"></param>
    /// <param name="nameRef"></param>
    /// <returns></returns>
    public static int GetItemByName(this ItemSetManager itemSetManager, int groupIndex, string nameRef)
    {
        var isg = itemSetManager.ItemSetGroups;
        var items = isg[groupIndex].ItemSetList;
        var item = items.FirstOrDefault(x => x.State.ToLower() == nameRef.ToLower());
        return item.Index;
    }
}
