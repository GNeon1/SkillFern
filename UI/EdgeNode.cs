/*
 * EdgeNode.cs
 * 
 * Extended UpgradeNode class to allows for multiple purchases of the node
 */

using SkillFern.Custom;
using SkillFern.Networking;
using SkillFern.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SkillFern.UI
{
    public class EdgeNode : UpgradeNode
    {

        private int timesPurchased; // amount of times this node has been purchased

        public void Awake()
        {
            base.Awake();
            cost = SkillDataManager.instance.baseEdgeNode;
            timesPurchased = 0;
        }

        /*
         * increment times purchased and increase price instead of marking owned when purchased
         */
        public override void setOwned(bool newOwned)
        {
            if (!newOwned)
                return;

            timesPurchased++;
            cost = SkillDataManager.instance.baseEdgeNode + SkillDataManager.instance.edgeNodeIncrement * timesPurchased;
        }

        /*
         * Hide cost text unless node is available
         */
        public override void UpdateStatus()
        {
            base.UpdateStatus();

            if (!CanPurchase())
                costText.enabled = false;
            else
                costText.enabled = true;
        }

    }
}
