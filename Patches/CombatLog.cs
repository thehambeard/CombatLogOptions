using CombatLogOptions.Utility;
using HarmonyLib;
using Kingmaker.UI.MVVM._PCView.CombatLog;

namespace CombatLogOptions.Patches
{
    internal class CombatLog
    {
        [HarmonyPatch(typeof(CombatLogItemPCView), (nameof(CombatLogItemPCView.BindViewImplementation)))]
        static class CombatLogItemPCView_Patch
        {
            [HarmonyPostfix]
            static void PostFix(CombatLogItemPCView __instance)
            {
                var i = CombatLogOptions.CombatLog.Controller.Instance;

                if (i != null)
                {
                    i.Builder.TextMeshes.Add(__instance.m_Text);
                    __instance.m_Text.SetUnderLay(i.HighlightColor, i.HighlightDilate, i.HighlightSoftness);
                }
            }
        }
    }
}
