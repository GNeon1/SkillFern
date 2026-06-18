/*
 * MenuPageSkills.cs
 * 
 * Behavior for the menu page displaying the skill fern
 * PAGE INDEX: 23
 */

using SkillFern.Custom;
using SkillFern.Utilities;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SkillFern.UI
{
    public class MenuPageSkills : CustomMenuPage
    {

        public int lineThickness = 5;
        public GameObject lineContainer; // an empty gameobject to parent node lines to
        public TextMeshProUGUI skillPointsText; // text to display current skill points

        [ContextMenu("Draw Lines")]
        public void DrawLines() {
            for (int i = lineContainer.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(lineContainer.transform.GetChild(i).gameObject);

            foreach (FernNode node in GetComponentsInChildren<FernNode>())
                node.DrawLines(lineContainer, lineThickness);
        }


        [HideInInspector]
        public static string PREFAB_NAME = "Menu Page Skills"; // name of the prefab to import for this menu page
        [HideInInspector]
        public static int CUSTOM_PAGE_INDEX = 23;

        private List<FernNode> nodes; // list of all nodes in the fern

        /*
         * set custom page index to 23 and initialize nodes
         */
        protected override void Start()
        {
            this.customPageIndex = CUSTOM_PAGE_INDEX;
            base.Start();

            skillPointsText.SetText("Points: " + SkillDataManager.instance.GetLocalSkillPoints());

            // automatically find all nodes
            nodes = new List<FernNode>(GetComponentsInChildren<FernNode>());

            DrawLines();

            // pull local node purchases and initialize each node
            List<string> purchasedIDs = SkillDataManager.instance.GetPlayerData(PlayerHelper.GetLocalSteamID()).ownedNodes;
            foreach (string id in purchasedIDs)
            {
                FernNode node = nodes.Find(n => n.nodeID == id);
                if (node != null)
                {
                    node.setOwned(true);
                }
            }

            foreach (FernNode node in nodes)
                node.Initialize();
        }

        /*
         * Every frame check if back key is pressed to exit the menu
         */
        private void Update()
        {
            if (SemiFunc.InputDown(InputKey.Back))
            {
                this.ButtonEventBack();
            }
        }

        /*
         * Updates the status of every node and the skill points
         */
        public void UpdateAllNodes(int pointsCorrection = -1) {

            skillPointsText.SetText("Points: " + (pointsCorrection == -1 ? SkillDataManager.instance.GetLocalSkillPoints() : pointsCorrection));

            foreach (FernNode node in nodes)
                node.UpdateStatus();
        }

        /*
         * Behavior for when the back button is pressed
         */
        public void ButtonEventBack()
        {
            // close the skills page and open the escape menu
            MenuManager.instance.PageCloseAll();
            MenuManager.instance.PageOpen(MenuPageIndex.Escape, false);
        }
    }
}
