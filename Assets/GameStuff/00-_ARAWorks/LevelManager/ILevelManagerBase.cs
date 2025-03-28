using ARAWorks.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.LevelManager
{
    public interface ILevelManagerBase
    {
        event Action<ContractLevel> BeforeLevelLoaded;
        event Action<ContractLevel> CoreLevelLoaded;
        event Action<ContractLevel> AllLevelsLoaded;
        event Action<ContractLevel> LevelUnloaded;
        ContractLevel CurrentLevel { get; }
        ContractLevel PreviousLevel { get; }
    }
}