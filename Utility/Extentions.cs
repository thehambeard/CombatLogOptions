using Kingmaker.Blueprints;
using Kingmaker.Visual.MaterialEffects;
using System;
using System.Collections.Generic;
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

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> baseDictionary,
            IDictionary<TKey, TValue> dictionaryToAdd)
        {
            dictionaryToAdd.ForEach(x => baseDictionary.Add(x.Key, x.Value));
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
