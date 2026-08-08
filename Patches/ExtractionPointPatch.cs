/*
 * ExtractionPointPatch.cs
 * 
 * Patches the extraction point to process custom shop items
 */

using HarmonyLib;
using SkillFern.Custom;
using SkillFern.Networking;
using SkillFern.Unity;
using SkillFern.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(ExtractionPoint))]
    public class ExtractionPointPatch
    {
        /*
         * BEFORE MenuManager.Start
         * 
         * Processes the skill cubes into skill points for the team
         */
        [HarmonyPatch("DestroyAllPhysObjectsInShoppingList")]
        [HarmonyPrefix]
        public static void ProcessSkillCubes(ExtractionPoint __instance)
        {
            if (SemiFunc.IsMasterClientOrSingleplayer())
            {
                // fetch the shopping list
                List<ItemAttributes> shoppingList = (List<ItemAttributes>)AccessTools.Field(typeof(ShopManager), "shoppingList").GetValue(ShopManager.instance);

                Plugin.LogInfo("Checking " + shoppingList.Count + " items");

                // cycle through all items on shopping list
                foreach (ItemAttributes itemAttributes in shoppingList)
                {
                    Plugin.LogInfo("Item is of type " + itemAttributes.item.itemType);
                    if (itemAttributes.item.itemType == (SemiFunc.itemType)SkillCube.SKILL_CUBE_INDEX)
                    {
                        SkillNetworkSync.UpdateSkillPointsForAll(((SkillCube)itemAttributes.item).skillPointValue);
                    }
                }
            }
        }
    }
}
