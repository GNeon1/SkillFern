/*
 * DebugHelper.cs
 *  
 * Contains a variety of helpful debug functions to call from the C# console in-game
 */

using SkillFern.Custom;

namespace SkillFern.Utilities
{
    public class DebugHelper
    {

        /*
         * Moves to a shop level with re-rolled items
         */
        public static void TestShop() {
            RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Shop);
        }

        /*
         * Test all upgrades by applying them to local player through network
         */
        public static void TestAllUpgrades() {
            Plugin.LogInfo("Testing all upgrades:");
            foreach (string skillName in SkillData.SKILL_NAMES) {
                Plugin.LogInfo("-- Trying " + skillName);
                SkillDataManager.instance.UpdateSkill(SkillFern.Utilities.PlayerHelper.GetLocalSteamID(), skillName, 2);
            }
        }
    }
}
