/*
 * UpgradeNode.cs
 * 
 * Extended FernNode class for nodes which bestow an upgrade when purchased
 */

using SkillFern.Custom;
using SkillFern.Networking;
using SkillFern.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SkillFern.UI
{
    public class UpgradeNode : FernNode
    {

        public SkillData.SKILL_TYPE upgradeType; // type of upgrade this node bestows
        public int upgradeAmount; // amount of the levels to upgrade

        /*
         * Bestows the upgrade associated with this node to the player when purchased
         */
        public override void Activate()
        {
            SkillNetworkSync.UpdateSkill(PlayerHelper.GetLocalSteamID(), upgradeType, upgradeAmount);
        }
    }
}
