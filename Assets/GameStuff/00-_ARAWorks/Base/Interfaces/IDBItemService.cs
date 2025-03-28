using ARAWorks.Base.Contracts;
using ARAWorks.Base.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Base.Interfaces
{
    public interface IDBItemService
    {
        ContractItem GetGlobalItemData(string globalItemID, ERarityType rarityType);
    }
}
