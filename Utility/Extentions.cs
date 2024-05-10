using Kingmaker.Blueprints;
using Kingmaker.Visual.MaterialEffects;
using System;
using TMPro;
using UnityEngine;

namespace CombatLogOptions.Utility
{
    internal static class Extensions
    {
        public static BlueprintGuid ToGUID(this string guid)
        {
            return new BlueprintGuid(Guid.Parse(guid));
        }

        public static void SetUnderLay(this TextMeshProUGUI textMesh, Color color, float dilate, float softness)
        {
            textMesh.enabled = false;

            var material = textMesh.fontMaterial;

            material.SetKeywordEnabled("UNDERLAY_ON", true);
            material.SetColor("_UnderlayColor", color);
            material.SetFloat("_UnderlayDilate", dilate);
            material.SetFloat("_UnderlaySoftness", softness);

            textMesh.enabled = true;
        }
    }
}
