/*
 * MenuPageSkills.cs
 * 
 * Behavior for the menu page displaying the skill fern
 * PAGE INDEX: 23
 */

using System.Collections.Generic;
using UnityEngine;

namespace SkillFern.UI
{
    public class MenuPageSkills : CustomMenuPage
    {

        public static string PREFAB_NAME = "Menu Page Skills"; // name of the prefab to import for this menu page
        public static int CUSTOM_PAGE_INDEX = 23;

        public List<FernNode> nodes; // list of all nodes in the fern

        /*
         * set custom page index to 23
         */
        protected override void Start()
        {
           this.customPageIndex = CUSTOM_PAGE_INDEX;
           base.Start();
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
