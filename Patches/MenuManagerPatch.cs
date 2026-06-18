/*
 * MenuManagerPatch.cs
 * 
 * Patches the menu manager to register custom menus when initialized
 */

using HarmonyLib;
using SkillFern.Custom;
using SkillFern.Utilities;
using UnityEngine;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(MenuManager))]
    public class MenuManagerPatch
    {
        /*
         * AFTER MenuManager.Start
         * 
         * Tells the asset helper to register custom menus
         */
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void RegisterMenus(MenuManager __instance)
        {
            Plugin.LogInfo("Waiting to register menus...");
            __instance.StartCoroutine(AssetHelper.RegisterMenus(__instance));
        }
    }
}
