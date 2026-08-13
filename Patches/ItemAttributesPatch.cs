/*
 * ItemAttributesPatch.cs
 * 
 * Sets price for SkillCubes
 */

using HarmonyLib;
using Photon.Pun;
using SkillFern.Custom;
using SkillFern.Unity;
using SkillFern.Utilities;
using System.Reflection;
using UnityEngine;
using static HarmonyLib.AccessTools;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(ItemAttributes))]
    public class ItemAttributesPatch
    {
        /*
         * AFTER ItemAttributes.Start
         * 
         * Sets min and max value to match skill cost
         */
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Initialize(ref float ___itemValueMin, ref float ___itemValueMax, Item ___item)
        {
            if (___item.itemType != (SemiFunc.itemType)SkillCube.SKILL_CUBE_INDEX)
                return;

            ___itemValueMin = (float)(ConfigHelper.SkillCubeCost() * ((SkillCube)___item).skillPointValue);
            ___itemValueMax = ___itemValueMin;
        }

        /*
         * BEFORE ItemAttributes.GetValue
         * 
         * if this item is a skill cube, set the cost to coorespond to its value
         */
        [HarmonyPatch("GetValue")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        public static bool SkillCubePrice(ref int ___value, PhotonView ___photonView, Item ___item)
        {
            // if not host, skip
            if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
                return true;

            // if not a skill cube, skip the rest
            if (___item.itemType != (SemiFunc.itemType)SkillCube.SKILL_CUBE_INDEX)
                return true;

            // set the price based on config and number of points
            ___value = (int)(ConfigHelper.SkillCubeCost() * ((SkillCube)___item).skillPointValue);
            ___value -= (int)(___value * ConfigHelper.DiscountPerPoint() * (((SkillCube)___item).skillPointValue - 1));
            if (GameManager.Multiplayer() && ___photonView.ViewID != 0)
            {
                 ___photonView.RPC("GetValueRPC", RpcTarget.Others, new object[]
                {
                    ___value
                });
            }

            // skip the normal value assignment
            return false;
        }
    }
}
