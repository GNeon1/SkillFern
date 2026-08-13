/*
 * SkillCube.cs
 * 
 * ScriptableObject for SkillCube shop items
 */

using UnityEngine;

namespace SkillFern.Unity
{

    [CreateAssetMenu(fileName = "SP Cube", menuName = "SkillFern/SkillCube")]
    public class SkillCube : Item
    {

        [HideInInspector]
        public static int SKILL_CUBE_INDEX = 25; // the index of the skill cube item type in the itemType enum

        public int skillPointValue = 1; // how many skill points this skill cube is worth
        public PrefabRef shopPrefab;

        /*
         * Set default values
         */
        private void Reset() {
            disabled = false;
            itemName = "SP Cube ";
            itemNameLocalized = null;
            description = "";
            itemType = (SemiFunc.itemType)SKILL_CUBE_INDEX;
            emojiIcon = SemiFunc.emojiIcon.drone_battery;
            itemVolume = SemiFunc.itemVolume.upgrade;
            maxAmount = 6;
            maxAmountInShop = 6;
            maxPurchaseAmount = 999;
            maxPurchase = false;
            physicalItem = false;
        }

    }
}
