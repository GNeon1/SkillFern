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
            if (!PlayerHelper.IsHost())
                return;

            SkillNetworkSync.saveCountdown = 0;

            Plugin.LogInfo("Awarding points. . .");
            foreach (string id in PlayerHelper.GetAllPlayerSteamIDs())
            {
                SkillNetworkSync.saveCountdown += 1;
                SkillNetworkSync.UpdateSkillPoints(id, 2);
            }
        }

    }
}
