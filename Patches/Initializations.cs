using CombatLogOptions.Utility;
using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using System.IO;

namespace CombatLogOptions.Patches
{
    internal class Initializations
    {
        [HarmonyPatch(typeof(BlueprintsCache))]
        static class BlueprintsCache_Patch
        {
            [HarmonyPriority(Priority.First)]
            [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
            static void Postfix()
            {
                Main.Logger.Log("Initializing...");
                AssetBundleManager.Initialize();
                AssetBundleManager.Instance.LoadAllBundles(Path.Combine(Main.ModPath, "Bundles"));
                Prefabs.Initialize();
            }
        }
    }
}
