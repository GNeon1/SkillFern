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
         * Checks if an entry in the itemsPurchased dictionary exists before trying to access it
         */
        [HarmonyPatch("ItemPurchase")]
        [HarmonyPrefix]
        public static bool ItemPurchase(string itemName)
        {
            // if it does not exist, create it
            if (!StatsManager.instance.itemsPurchasedTotal.ContainsKey(itemName))
            {
                StatsManager.instance.itemsPurchased.Add(itemName, 0);
                StatsManager.instance.itemsPurchasedTotal.Add(itemName, 0);
            }

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
