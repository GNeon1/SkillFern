/*
 * StatsManagerPatch.cs
 * 
 * The stats manager keeps track of run stats, interacts with save data, and generates item upgrades for the shop
 */

using BepInEx;
using HarmonyLib;
using SkillFern.Custom;
using SkillFern.Utilities;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(StatsManager))]
    public class StatsManagerPatch
    {
        /*
         * AFTER StatsManager.Start
         * 
         * Registers skill cubes
         */
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void RegisterSkillCubes(StatsManager __instance)
        {
            Plugin.LogInfo("Waiting to register skill cubes...");
            __instance.StartCoroutine(AssetHelper.RegisterSkillCubes());
        }

        /*
         * BEFORE StatsManager.ItemPurchase
         * 
         * Skip the item if it is a skill cube
         */
        [HarmonyPatch("ItemPurchase")]
        [HarmonyPrefix]
        public static bool ItemPurchase(string itemName)
        {
            Plugin.LogInfo("Purchasing item: " + itemName);
            Plugin.LogInfo("Physical: " + StatsManager.instance.itemDictionary[itemName].physicalItem);

            // if it does not exist, create it
            if (itemName.Substring(0, 7) == "SP Cube")
                return false;

            return true;
        }

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

    }
}
