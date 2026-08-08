/*
 * ShopHelper.cs
 * 
 * Contains all Shop-related functions and values
 */

using SkillFern.Unity;
using UnityEngine;
using System.Collections.Generic;

namespace SkillFern.Utilities
{
    public class ShopHelper
    {

        public static SkillCube[] skillCubes; // array of loaded skill cube items
        private static int[] cubeValues = { 1, 3, 5 };

        /*
         * Adds the correct number and type of skill cubes to the shop
         * 
         * @param potentialItemUpgrades - the list of items to be added to the shop
         */
        public static void SpawnSkillCubes(List<Item> potentialItemUpgrades)
        {

            int moonsCompleted = (SemiFunc.RunGetLevelsCompleted()/ConfigHelper.LevelsPerInterval() + 1);
            int pointsInShop = Random.Range(ConfigHelper.MinShopPointsPerInterval() * moonsCompleted, Mathf.Min(ConfigHelper.MaxShopPointsPerInterval() * moonsCompleted + 1, ConfigHelper.ShopPointsCap()));

            while (pointsInShop > 0)
            {
                int minPossible = 0;
                if (pointsInShop > 9)
                    minPossible = 2;
                else if (pointsInShop > 4)
                    minPossible = 1;

                int possible = Random.Range(minPossible, 3);

                if (cubeValues[possible] > pointsInShop)
                    continue;

                pointsInShop -= cubeValues[possible];
                potentialItemUpgrades.Add(skillCubes[possible]);
            }
        }

    }
}
