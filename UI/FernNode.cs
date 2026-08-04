/*
 * FernNode.cs
 * 
 * Represents a single node in the skill fern
 * Must be attached to each node UI element
 */

using HarmonyLib;
using SkillFern.Custom;
using SkillFern.Networking;
using SkillFern.Utilities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SkillFern.UI
{
    public class FernNode : MonoBehaviour
    {

        private class LineContainer {
            public GameObject lineObject;
            public FernNode dependency;
        }

        private List<LineContainer> lines = new List<LineContainer>();

        public string nodeID; // unique ID for the node
        public Image[] targetImages; // the image component to change the color of when purchased
        public TextMeshProUGUI costText; // text to display current skill points
        public FernNode[] dependencies; // if any of these nodes is purchased, this node is unlocked
        public int cost; // cost of the node in skill points

        protected bool owned; // whether this node has been purchased yet
        protected MenuButton button; // the button attached to this object

        protected bool wasHovering; // whether the mouse was hovering over this node last update (for clearing hover text)

        private MenuPageSkills skillsPage; // parent skills page

        public bool isOwned() { return owned; }
        public virtual void setOwned(bool newOwned) { owned = newOwned; }

        /*
         * Draw a line to each of this node's dependencies
         */
        public void DrawLines(GameObject lineContainer, int lineThickness) {

            if (Application.isEditor)
            {
                Debug.Log(gameObject.name);
                nodeID = System.Guid.NewGuid().ToString();

                var images = transform.GetComponentsInChildren<Image>().ToList<Image>();
                images.RemoveAt(0);
                targetImages = images.ToArray();

                transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("");
                costText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                costText.SetText("" + cost);
            }

            if (dependencies == null || dependencies.Length == 0)
                return;

            // create new list of lines
            lines = new List<LineContainer>();

            // draw a line to each dependency
            for (int i = 0; i < dependencies.Length; i++)
            {
                // create new line
                LineContainer container = new LineContainer();
                container.lineObject = new GameObject("Fern Line");
                container.dependency = dependencies[i];

                // parent the line
                container.lineObject.transform.SetParent(lineContainer.transform, false);

                // attach image component to line
                Image lineImage = container.lineObject.AddComponent<Image>();
                lineImage.color = Color.white;

                // set the line pivot to the left center so we can rotate and scale it from the starting point
                RectTransform lineRect = lineImage.rectTransform;
                lineRect.pivot = new Vector2(0f, 0.5f);

                // find the points to attach the line to
                RectTransform pointA = GetComponent<RectTransform>();
                RectTransform pointB = dependencies[i].GetComponent<RectTransform>();

                Vector3 centerA = pointA.TransformPoint(pointA.rect.center);
                Vector3 centerB = pointB.TransformPoint(pointB.rect.center);

                // move the line to the center of this node
                lineRect.position = centerA;

                // calculate line length
                float distance = Vector3.Distance(centerA, centerB);

                // set the line size
                lineRect.sizeDelta = new Vector2(distance, lineThickness);

                // angle the line
                Vector3 direction = centerB - centerA;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                lineRect.rotation = Quaternion.Euler(0, 0, angle);

                lines.Add(container);
            }

        }

        public void OnValidate() {
            costText.SetText("" + cost);
        }

        /*
         * Assign a unique ID to this node in editor
         */
        public virtual void Reset() {
            cost = 1;
            nodeID = System.Guid.NewGuid().ToString();

            var images = transform.GetComponentsInChildren<Image>().ToList<Image>();
            images.RemoveAt(0);
            targetImages = images.ToArray();

            dependencies = new FernNode[1];
            costText = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            costText.SetText("" + cost);
        }

        public void Awake() {
            button = GetComponent<MenuButton>();
            skillsPage = GetComponentInParent<MenuPageSkills>();
        }

        /*
         * Manage hover behavior on update
         */
        public void Update() {
            // if hovering, tell skills page to show display string
            if ((int)AccessTools.Field(typeof(MenuButton), "buttonState").GetValue(button) <= 1 && !owned)
            {
                wasHovering = true;
                skillsPage.SetHoverText(GetDisplayString());
            } else if (wasHovering) // if cleanup is needed, clear the hover text
            {
                wasHovering = false;
                skillsPage.ClearHoverText();
            }
        }

        /*
         * Initializes this node
         */
        public virtual void Initialize() {
            // update the color based on ownership
            UpdateStatus();
        }

        /*
         * @returns whether this node is ready to be purchased
         */
        public bool CanPurchase() {
            return !owned && (dependencies.Length == 0 || dependencies.Any(node => node.isOwned()));
        }

        /*
         * Purchases the node for the player
         */
        public void Purchase() {
            int currentPoints = SkillDataManager.instance.GetLocalSkillPoints();

            if (currentPoints < cost)
                return;

            Plugin.LogInfo($"Purchased node {nodeID}");

            setOwned(true);

            SkillNetworkSync.UpdateSkillPoints(PlayerHelper.GetLocalSteamID(), -cost);

            skillsPage.UpdateAllNodes(currentPoints - cost);

            Activate();

            SkillNetworkSync.PurchaseNode(PlayerHelper.GetLocalSteamID(), nodeID);
        }

        /*
         * Activates the node's effect
         */
        public virtual void Activate()
        {
            Plugin.LogInfo($"Node {nodeID} has no effects");
        }

        /*
         * Update the color and button status of this node based on whether it is purchased
         */
        public virtual void UpdateStatus()
        {
            // show or hide cost text
            if (owned)
                costText.enabled = false;
            else
                costText.SetText("" + cost);

            // enable or disable button
            if (!owned && CanPurchase())
                button.enabled = true;
            else
                button.enabled = false;

            foreach (Image targetImage in targetImages)
                // set color
                if (owned)
                    targetImage.color = Color.green;
                else if (CanPurchase())
                    targetImage.color = Color.white;
                else
                    targetImage.color = Color.gray;

            // set color of dependency lines
            foreach (LineContainer line in lines)
            {
                if (owned && line.dependency.isOwned())
                    line.lineObject.GetComponent<Image>().color = Color.green;
                else if (owned || line.dependency.isOwned())
                    line.lineObject.GetComponent<Image>().color = Color.white;
                else
                    line.lineObject.GetComponent<Image>().color = Color.gray;
            }
        }

        /*
         * @returns hover text to display for this node
         */
        public virtual string GetDisplayString() { return "[BLANK]"; }
    }
}
