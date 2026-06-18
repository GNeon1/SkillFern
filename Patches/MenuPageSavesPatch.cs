/*
 * MenuPageEscPatch.cs
 * 
 * Patches the escape menu to add the skillfern button
 */

using HarmonyLib;
using SkillFern.Custom;
using SkillFern.Utilities;
using UnityEngine;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(MenuPageSaves))]
    public class MenuPageSavesPatch
    {
        /*
         * BEFORE MenuPageSaves.OnNewGame
         * 
         * Prepares a new SkillDataManager when a new save is started
         */
        [HarmonyPatch("OnNewGame")]
        [HarmonyPrefix]
        public static void OnNewGame()
        {
            SkillDataManager.instance = new SkillDataManager();
        }
    }
}
