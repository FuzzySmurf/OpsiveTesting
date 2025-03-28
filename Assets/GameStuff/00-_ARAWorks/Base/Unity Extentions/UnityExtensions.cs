using ARAWorks.Base.Contracts;
using ARAWorks.Base.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARAWorks.Base.Extensions
{
    public static class UnityExtensions
    {
        private static System.Random rng = new System.Random();

        /// <summary>
        /// Shuffles this collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Extends all arrays with a method for adding on a second array
        /// </summary>
        public static T[] Concatenate<T>(this T[] first, T[] second)
        {
            if (first == null)
            {
                return second;
            }
            if (second == null)
            {
                return first;
            }

            return first.Concat(second).ToArray();
        }

        /// <summary>
        /// Extends List of Vector3 to convert vertices to triangles
        /// </summary>
        /// <param name="verticies">The vertices to convert into triangles</param>
        /// <returns>Returns a List of Triangles</returns>
        public static List<Triangle> Triangulate(this List<Vector3> verticies)
        {
            List<Vector2> vertices2D = new List<Vector2>();
            for (int i = 0; i < verticies.Count; i++)
                vertices2D.Add(new Vector2(verticies[i].x, verticies[i].z));

            Triangulator tr = new Triangulator(vertices2D.ToArray());
            int[] indicies = tr.Triangulate();

            List<Triangle> tris = new List<Triangle>();
            for (int i = 0; i < indicies.Length; i += 3)
            {
                Triangle newTri = new Triangle(verticies[indicies[i]], verticies[indicies[i + 1]], verticies[indicies[i + 2]]);
                tris.Add(newTri);
            }
            return tris;
        }

        /// <summary>
        /// Used to create a new instance for a singleton that will persist between scenes if necessary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">the instance</param>
        /// <param name="newInstance">the object calling to see if it should be the instance</param>
        /// <param name="gameObject">the object with newInstance on it</param>
        /// <returns></returns>
        public static T CreatePersistentInstance<T>(T instance, T newInstance, GameObject gameObject)
            where T : class
        {
            if (instance == null)
            {
                GameObject.DontDestroyOnLoad(gameObject.transform.root.gameObject);
                return newInstance;
            }
            else
            {
                GameObject.Destroy(gameObject);
                return null;
            }
        }

        /// <summary>
        /// Used to create a new instance for a singleton that will not persist betweeen scenes if necessary 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">the instance</param>
        /// <param name="newInstance">the object calling to see if it should be the instance</param>
        /// <param name="gameObject">the object with newInstance on it</param>
        /// <returns></returns>
        public static T CreateNonpersistentSingleton<T>(T instance, T newInstance, GameObject gameObject)
            where T : class
        {
            if (instance != null && instance != newInstance)
                GameObject.Destroy(gameObject);

            return newInstance;
        }

        public static Texture2D ToTexture2D(this RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }

        public static Vector2 MouseWorldToLocal(this VisualElement ve)
        {
            //Vector2 mousePosition = InputManager.Instance.PlayerControlsHandler.UINavigation.Point.ReadValue<Vector2>();
            //Vector2 mousePosition = new Vector2(0, 0);
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePositionAltered = new Vector2(mousePosition.x / Screen.width, 1 - (mousePosition.y / Screen.height));

            return ve.WorldToLocal(mousePositionAltered * ve.panel.visualTree.layout.size);
        }

        /// <summary>
        /// Performs more efficient distance check to determine whether pos2 is within maxDis of pos1
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="maxDis"></param>
        /// <returns></returns>
        public static bool WithinDistanceOf(this Vector3 pos1, Vector3 pos2, float maxDis)
        {
            float disFromOrigin = (pos1 - pos2).sqrMagnitude;
            if (disFromOrigin <= maxDis * maxDis)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Sort a RaycastHit array by distance
        /// </summary>
        /// <param name="hits"></param>
        /// <param name="amountHit"></param>
        public static void SortByDistance(this RaycastHit[] hits, int amountHit)
        {
            for (int j = amountHit - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    if (hits[i].distance > hits[i + 1].distance)
                    {
                        RaycastHit cachedHit = hits[i];
                        hits[i] = hits[i + 1];
                        hits[i + 1] = cachedHit;
                    }
                }
            }
        }

        /// <summary>
        /// Sort a Vector2 array by Dot product of given direction
        /// </summary>
        /// <param name="dirs"></param>
        /// <param name="compareDir"></param>
        public static void SortByDot(this Vector2[] dirs, Vector2 compareDir)
        {
            int amount = dirs.Length;
            for (int j = amount - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    if (Vector2.Dot(dirs[i], compareDir) < Vector2.Dot(dirs[i + 1], compareDir))
                    {
                        Vector2 cachedDir = dirs[i];
                        dirs[i] = dirs[i + 1];
                        dirs[i + 1] = cachedDir;
                    }
                }
            }
        }

        public static Dictionary<EAttributeTypes, int> GetMap(this ContractAttributes ca)
        {
            return new Dictionary<EAttributeTypes, int>()
            {
                {EAttributeTypes.Strength, ca.Strength},
                {EAttributeTypes.Agility, ca.Agility},
                {EAttributeTypes.Constitution, ca.Constitution},
                {EAttributeTypes.Intelligence, ca.Intelligence},
                {EAttributeTypes.Wisdom, ca.Wisdom}
            };
        }

        public static Dictionary<EStatTypes, float> GetMap(this ContractStatsCore csc)
        {
            return new Dictionary<EStatTypes, float>()
            {
                {EStatTypes.ArmorRating, csc.ArmorRating},
                {EStatTypes.PhysicalDamage, csc.PhysicalDamage},
                {EStatTypes.ArcaneDamage, csc.ArcaneDamage},
                {EStatTypes.PhysicalResistance, csc.PhysicalResistance},
                {EStatTypes.ArcaneResistance, csc.ArcaneResistance},
                {EStatTypes.CriticalDamage, csc.CriticalDamage},
                {EStatTypes.CriticalChance, csc.CriticalChance},
                {EStatTypes.MaxHealth, csc.MaxHealth},
                {EStatTypes.MoveSpeed, csc.MoveSpeed},
                {EStatTypes.AttackSpeed, csc.AttackSpeed},
                {EStatTypes.HealthRegeneration, csc.HealthRegeneration},
                {EStatTypes.HealingPower, csc.HealingPower}
            };
        }


        #region EquipmentSlotType Enum Extensions

        //A weapon can be 20 to 29
        /// <summary>
        /// Checks if this is a weapon type.
        /// </summary>
        /// <param name="equipmentSlotType"></param>
        /// <returns>True if this is a weapon type.</returns>
        public static bool IsWeapon(this EEquipmentSlotType equipmentSlotType)
        {
            int type = (int)equipmentSlotType;
            return type >= 20 && type < 30;
        }

        //An Armor can be 0 to 10 
        /// <summary>
        /// Checks if this is an armor type.
        /// </summary>
        /// <param name="equipmentSlotType"></param>
        /// <returns>True if this an armor type.</returns>
        public static bool IsArmor(this EEquipmentSlotType equipmentSlotType)
        {
            int type = (int)equipmentSlotType;
            return type >= 0 && type < 10;
        }


        //An accessory can be 10 to 19
        /// <summary>
        /// Checks if this is an accessory.
        /// </summary>
        /// <param name="equipmentSlotType"></param>
        /// <returns>True if this is an accesory.</returns>
        public static bool IsAccessory(this EEquipmentSlotType equipmentSlotType)
        {
            int type = (int)equipmentSlotType;
            return type >= 10 && type < 20;
        }

        #endregion

        #region Stats Enum Extensions

        //A Core stat can be 0 to 100
        /// <summary>
        /// Checks if this stat is a Core stat
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static bool IsCore(this EStatTypes stats)
        {
            int type = (int)stats;
            return type >= 0 && type <= 100;
        }

        //A Elemental Damage stat can be 101 to 200
        /// <summary>
        /// Checks if this stat is a Elemental stat
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static bool IsElementalDamage(this EStatTypes stats)
        {
            int type = (int)stats;
            return type >= 101 && type <= 200;
        }

        //A Elemental Resistance stat can be 201 to 300
        /// <summary>
        /// Checks if this stat is a Elemental stat
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static bool IsElementalResistance(this EStatTypes stats)
        {
            int type = (int)stats;
            return type >= 201 && type <= 300;
        }

        //A Utility stat can be 301+
        /// <summary>
        /// Checks if this stat is a Utility stat
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static bool IsUtility(this EStatTypes stats)
        {
            List<int> a = new List<int>();
            int type = (int)stats;
            return type >= 301;
        }

        /// <summary>
        /// Converts stat from Resistance to corresponding Damage type. If no correspodning type found, returns null.
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static EStatTypes? ResistanceToDamage(this EStatTypes stats)
        {
            if (stats == EStatTypes.PhysicalResistance)
            {
                return EStatTypes.PhysicalDamage;
            }
            else if (stats == EStatTypes.ArcaneResistance)
            {
                return EStatTypes.ArcaneDamage;
            }
            else if (stats.IsElementalResistance() == true)
            {
                return (EStatTypes)((int)stats - 100);
            }

            return null;
        }
        /// <summary>
        /// Converts stat from Damage to corresponding Resistance type. If no correspodning type found, returns null.
        /// </summary>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static EStatTypes? DamageToResistance(this EStatTypes stats)
        {
            if (stats == EStatTypes.PhysicalDamage)
            {
                return EStatTypes.PhysicalResistance;
            }
            else if (stats == EStatTypes.ArcaneDamage)
            {
                return EStatTypes.ArcaneResistance;
            }
            else if (stats.IsElementalDamage() == true)
            {
                return (EStatTypes)((int)stats + 100);
            }

            return null;
        }

        #endregion

        public static ContractAttributes CombineAttributes(ContractAttributes left, ContractAttributes right)
        {
            ContractAttributes ret = new ContractAttributes();
            ret.Strength = left.Strength + right.Strength;
            ret.Agility = left.Agility + right.Agility;
            ret.Constitution = left.Constitution + right.Constitution;
            ret.Intelligence = left.Intelligence + right.Intelligence;
            ret.Wisdom = left.Wisdom + right.Wisdom;
            return ret;
        }

        public static ContractStatsCore CombineStatsCore(ContractStatsCore left, ContractStatsCore right)
        {
            ContractStatsCore ret = new ContractStatsCore();
            ret.ArmorRating = left.ArmorRating + right.ArmorRating;
            ret.PhysicalDamage = left.PhysicalDamage + right.PhysicalDamage;
            ret.ArcaneDamage = left.ArcaneDamage + right.ArcaneDamage;
            ret.PhysicalResistance = left.PhysicalResistance + right.PhysicalResistance;
            ret.ArcaneResistance = left.ArcaneResistance + right.ArcaneResistance;
            ret.CriticalDamage = left.CriticalDamage + right.CriticalDamage;
            ret.CriticalChance = left.CriticalChance + right.CriticalChance;
            ret.MaxHealth = left.MaxHealth + right.MaxHealth;
            ret.MoveSpeed = left.MoveSpeed + right.MoveSpeed;
            ret.AttackSpeed = left.AttackSpeed + right.AttackSpeed;
            ret.HealthRegeneration = left.HealthRegeneration + right.HealthRegeneration;
            ret.HealingPower = left.HealingPower + right.HealingPower;

            return ret;
        }

        public static List<ContractStatUtility> CombineStatUtility(List<ContractStatUtility> left, List<ContractStatUtility> right)
        {
            List<ContractStatUtility> ret = new List<ContractStatUtility>();

            for (int i = 0; i < left.Count; i++)
            {
                ContractStatUtility statUtility = new ContractStatUtility();
                statUtility.StatType = left[i].StatType;
                statUtility.Value = left[i].Value + right[i].Value;
                ret.Add(statUtility);
            }

            return ret;
        }
    }
}