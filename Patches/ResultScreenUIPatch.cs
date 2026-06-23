/*
 * SemiFuncPatch.cs
 * 
 * TODO: Description
 */

using BepInEx;
using HarmonyLib;
using SkillFern.Custom;
using SkillFern.Networking;
using SkillFern.Utilities;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(ResultScreenUI))]
    public class ResultScreenUIPatch
    {
        /*
         * TODO: description
         * 
         * Awards skill points at end of level
         */
        [HarmonyPatch("FinishResultScreen")]
        [HarmonyPostfix]
        static void AwardSkillPoints()
        {
            // if the player is not the host, they do not assign points
            if (!PlayerHelper.IsHost())
                return;

            // award points per player
            Plugin.LogInfo("Awarding points. . .");
            SkillNetworkSync.AwardSkillPointsForLevel();

            
        }

    }
}
