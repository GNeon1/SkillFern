/*
 * Plugin.cs
 *  
 * Initializes the mod and applies all patches on load
 */

using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace SkillFern
{
    [BepInPlugin("com.dajadeninja.repo.skillfern", "Skill Fern", "0.0.0")]
    public class Plugin : BaseUnityPlugin
    {

        private Harmony harmony; // Harmony instance to patch with

        /*
         * Load all patches when the plugin wakes up
         */
        private void Awake()
        {
            // instantiate Harmony
            harmony = new Harmony("com.dajadeninja.repo.skillfern");

            // announce success and next step
            Logger.LogInfo("Skill Fern loaded successfully! Loading patches. . .");

            // apply all patches
            harmony.PatchAll();
        }

        /*
         * Unpatch when a hot-reload goes through
         */
        private void OnDestroy()
        {
            Logger.LogInfo("Skill Fern unloading. Cleaning up patches...");

            harmony?.UnpatchSelf();
        }
    }
}
