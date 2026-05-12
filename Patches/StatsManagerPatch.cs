/*
 * StatsManagerPatch.cs
 * 
 * The stats manager keeps track of run stats, interacts with save data, and generates item upgrades for the shop
 */

using BepInEx;
using HarmonyLib;
using SkillFern.Custom;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(StatsManager))]
    public class StatsManagerPatch
    {

        /*
         * AFTER StatsManager.SaveGame
         * 
         * Saves skill data in parallel
         * 
         * @param filename - the name of the save being updated (no extension)
         */
        [HarmonyPatch("SaveGame")]
        [HarmonyPostfix]
        static void SaveSkills(string fileName)
        {
            SaveManager.Save(fileName);
        }

        /*
         * AFTER StatsManager.LoadGame
         * 
         * Loads skill data from existing save or makes one
         * 
         * @param filename - the name of the save being loaded (no extension)
         */
        [HarmonyPatch("LoadGame")]
        [HarmonyPostfix]
        static void LoadSkills(string fileName)
        {
            SaveManager.Load(fileName);
        }

        /*
         * AFTER StatsManager.RunStartStats
         * 
         * Initializes everything for the skill system at the start of a run
         */
        [HarmonyPatch("RunStartStats")]
        [HarmonyPostfix]
        static void RunSetup()
        {
            // initialize the SkillDataManager to ensure one exists
            SkillDataManager.Initialize();
        }

    }
}
