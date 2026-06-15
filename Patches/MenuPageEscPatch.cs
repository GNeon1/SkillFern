/*
 * MenuPageEscPatch.cs
 * 
 * Patches the escape menu to add the skillfern button
 */

using HarmonyLib;
using SkillFern.Utilities;
using UnityEngine;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(MenuPageEsc))]
    public class MenuPageEscPatch
    {
        /*
         * AFTER MenuPageEsc.Start
         * 
         * Adds the skillfern button to the escape menu
         */
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void AddSkillsButton(MenuPageEsc __instance)
        {
            // instantiate the button and parent it to the escape menu
            GameObject button = GameObject.Instantiate(AssetHelper.GetPrefab("Menu Button - Skills"), __instance.transform);
        }
    }
}
