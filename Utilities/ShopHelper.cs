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

        private const int MIN_ONES = 2; // the minimum number of cubes that must be spawned as ones

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
            int totalPoints = Mathf.Min(Random.Range(ConfigHelper.MinShopPointsPerInterval() * moonsCompleted, Mathf.Min(ConfigHelper.MaxShopPointsPerInterval() * moonsCompleted + 1, ConfigHelper.ShopPointsCap())), ConfigHelper.ShopPointsCap());
            int pointsLeft = totalPoints;
            int onesLeft = MIN_ONES;

            // 14 = 11 = 6 = 3 = 0
            // 14 = 14 = 9 = 6 = 3
            // 1 1 1 5 3 3

            Plugin.LogInfo("Distributing " + totalPoints + " points into shop");

            while (totalPoints > 0)
            {
                Plugin.LogInfo(" - Starting Cycle | totalPoints: " + totalPoints + " | pointsLeft: " + pointsLeft);

                int minPossible = 0;
                if (pointsLeft > 10)
                    minPossible = 2;
                else if (pointsLeft > 4 && totalPoints >= cubeValues[1])
                    minPossible = 1;

                int possible = Random.Range(minPossible, 3);

                if (onesLeft > 0)
                {
                    onesLeft--;
                    possible = 0;
                }
                else
                {
                    if (cubeValues[possible] <= totalPoints)
                        pointsLeft -= cubeValues[possible];
                }

                if (cubeValues[possible] > totalPoints)
                    continue;

                totalPoints -= cubeValues[possible];

                Plugin.LogInfo(" - - Adding a " + cubeValues[possible] + "SP cube");

                potentialItemUpgrades.Add(skillCubes[possible]);
            }
        }

    }
}
