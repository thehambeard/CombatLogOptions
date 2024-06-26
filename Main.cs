﻿using CombatLogOptions.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace CombatLogOptions
{
#if (DEBUG)
    [EnableReloading]
#endif
    internal static class Main
    {
        public static string ModPath { get; private set; }
        public static Utility.Logger Logger;
        public static ModEventHandler ModEventHandler;

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            ModPath = modEntry.Path;

            Logger = new(modEntry.Logger);
            ModEventHandler = new();

            modEntry.OnToggle = OnToggle;
#if (DEBUG)
            modEntry.OnGUI = OnGUIDebug;
            modEntry.OnUnload = Unload;
            return true;
        }

        private static bool Unload(UnityModManager.ModEntry modEntry)
        {
            ModEventHandler.Disable(modEntry, true);
            AssetBundleManager.Instance?.UnloadAllBundles();
            return true;
        }

#else
            modEntry.OnGUI = OnGUI;
            return true;
        }
#endif

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (value)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                ModEventHandler.Enable(modEntry, assembly);
            }
            else
            {
                ModEventHandler.Disable(modEntry);
            }

            return true;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {

        }

        private static void OnGUIDebug(UnityModManager.ModEntry modEntry)
        {
            if (GUILayout.Button("Load Bundle"))
            {
                AssetBundleManager.Initialize();
                AssetBundleManager.Instance.LoadAllBundles(Path.Combine(ModPath, "Bundles"));
                Prefabs.Initialize();
            }

            OnGUI(modEntry);
        }
    }
}