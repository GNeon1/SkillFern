/*
 * RunManagerPatch.cs
 * 
 * Manages the game state and level loading
 */

using BepInEx;
using HarmonyLib;
using SkillFern.Custom;
using SkillFern.Networking;
using SkillFern.Utilities;
using System.Collections.Generic;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(RunManager))]
    public class RunManagerPatch
    {
        /*
         * BEFORE RunManager.ResetProgress
         * 
         * Reset skill progress at end of game
         */
        [HarmonyPatch("ResetProgress")]
        [HarmonyPrefix]
        static void ResetSkillDataManager()
        {
            SkillDataManager.instance = new SkillDataManager();
        }

        /*
         * AFTER RunManager.MoonGetAttributes
         * 
         * Attaches skill points earned to the moon attributes returned
         */
        [HarmonyPatch("MoonGetAttributes")]
        [HarmonyPostfix]
        static void SkillPointsMessage(ref List<Moon.MoonAttribute> __result)
        {
            string text = "Earn " + SkillDataManager.CalculatePointsEarned(1) + " skill points per level.";

            foreach (Moon.MoonAttribute att in __result)
                if (att.text == text)
                    return;

            __result.Add(new Moon.MoonAttribute()
            {
                text = text,
                LocalizedText = null
            });
        }

    }
}
