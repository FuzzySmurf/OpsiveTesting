using UnityEngine;

namespace ARAWorks.Base.Extensions
{
    public static class InterfaceHelper
    {
        public static bool IsNull<T>(T inter) where T : class
        {
            if (typeof(T).IsInterface == false)
            {
                Debug.LogError("InterfaceHelper::IsNull -- Trying to check if an interface reference is null but a class reference was provided instead.");
                return false;
            }
            return inter == null || inter.Equals(null) == true;
        }
    }
}