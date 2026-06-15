/*
 * Plugin.cs
 *  
 * Initializes the mod and applies all patches on load
 */

using BepInEx;
using HarmonyLib;
using SkillFern.Networking;
using SkillFern.Utilities;
using UnityEngine;

namespace SkillFern
{
    [BepInPlugin("com.dajadeninja.repo.skillfern", "Skill Fern", "0.0.0")]
    public class Plugin : BaseUnityPlugin
    {

        public static Plugin instance;
        private Harmony harmony; // Harmony instance to patch with

        /*
         * Load all patches when the plugin wakes up
         */
        private void Awake()
        {
            instance = this;

            // instantiate Harmony
            harmony = new Harmony("com.dajadeninja.repo.skillfern");

            // announce success and next step
            LogInfo("Loaded successfully! Loading patches. . .");

            // apply all patches
            harmony.PatchAll();
            LogInfo("Patches loaded");

            // start listening for network traffic
            SkillNetworkSync.Initialize();
            LogInfo("Network initialized");

            // load the assets from bundles
            AssetHelper.LoadBundles();
        }

        /*
         * Unpatch when a hot-reload goes through
         */
        private void OnDestroy()
        {
            Logger.LogInfo("Skill Fern unloading. Cleaning up patches...");

            harmony?.UnpatchSelf();
        }

        public static void LogInfo(string msg)
        {
            instance.Logger.LogInfo($"Skill Fern: {msg}");
        }

        public static void LogError(string msg)
        {
            instance.Logger.LogError($"Skill Fern: {msg}");
        }
    }
}
