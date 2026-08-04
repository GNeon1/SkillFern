/*
 * Plugin.cs
 *  
 * Initializes the mod and applies all patches on load
 */

using BepInEx;
using HarmonyLib;
using SkillFern.Custom;
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

            instance.Logger.LogInfo("Setting up configuration. . .");
            ConfigHelper.Initialize(Config);

            // announce success and next step
            LogInfo("Loaded successfully! Loading patches. . .");

            // apply all patches
            harmony.PatchAll();
            LogInfo("Patches loaded");

            // load the assets from bundles
            AssetHelper.LoadBundles();

            // setup configuration
            

            Plugin.LogInfo("Newest 19");
        }

        public static void LogInfo(string msg)
        {
            if (ConfigHelper.EnableDebug())
                instance.Logger.LogInfo($"Skill Fern: {msg}");
        }

        public static void LogError(string msg)
        {
            if (ConfigHelper.EnableDebug())
                instance.Logger.LogError($"Skill Fern: {msg}");
        }
    }
}
