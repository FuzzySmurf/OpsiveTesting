using ARAWorks.Base.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageNumbersController 
{
    /// <summary>
    /// Event when an entity is damaged. Returned the damage info.
    /// </summary>
    event Action<ContractDamageInfo> OnEntityDamaged;

    void InvokeEntityDamage(ContractDamageInfo entityDamageInfo);
}
