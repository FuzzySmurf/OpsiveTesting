using System;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace ARAWorks.Base.Timer
{
    public class TimerMemoryManagementContainer
    {
        public readonly string ReferenceName;
        public TimerMemoryManagementType ManagementType { get; private set; }
        public readonly WeakReference Reference;

        public TimerMemoryManagementContainer(TimerMemoryManagementType type, UObject linkedReference = null)
        {
            ManagementType = type;
            Reference = new WeakReference(linkedReference);

            if (ManagementType == TimerMemoryManagementType.ClearOnObjectNullOrSceneUnload && linkedReference == null)
            {
                throw new UnityException($"Unable to manage memory effectively! Management Type chosen \"MemoryManagementType.ClearOnObjectNull\" but \"linkedReference\" is null. Please set a reference to monitor.");
            }
            if (linkedReference != null)
            {
                ReferenceName = linkedReference.name;
            }
        }
    }
}