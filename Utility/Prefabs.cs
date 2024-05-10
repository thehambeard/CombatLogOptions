using CombatLogOptions.Utility.Extentions;
using Owlcat.Runtime.Core.Utils;
using System.Linq;
using UnityEngine;

namespace CombatLogOptions.Utility
{
    internal static class Prefabs
    {
        public static GameObject CombatLogOptions => Get("CombatLogOptions");
        public static GameObject HeaderTemplate => Get("HeaderTemplate");
        public static GameObject SliderGroupTemplate => Get("SliderGroupTemplate");
        public static GameObject ToggleGroupTemplate => Get("ToggleGroupTemplate");

        public static void Initialize()
        {
            RemoveReferences();
        }

        public static GameObject Get(string name)
        {
            return AssetBundleManager.Instance.Prefabs[name];
        }

        private static void RemoveReferences()
        {
            var fabs = AssetBundleManager.Instance.Prefabs;
            var fabNames = fabs.Keys.ToList();

            foreach (var fab in fabs.Values)
            {
                foreach (var fabName in fabNames)
                {
                    if (fab.name == fabName)
                    {
                        fab.transform.SetParent(null);
                        continue;
                    }

                    Transform result;

                    while ((result = fab.transform.FindChildRecursive(fabName)) != null)
                        result.SafeDestroy();
                }
            }
        }
    }
}
