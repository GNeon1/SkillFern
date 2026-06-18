/*
 * CustomMenuPage.cs
 * 
 * Base class for all new menu pages added with this mod
 */

using System.Collections.Generic;
using UnityEngine;

namespace SkillFern.UI
{
    public class CustomMenuPage : MonoBehaviour
    {
        [HideInInspector]
        public static CustomMenuPage instance; // a static instance of this page
        [HideInInspector]
        public int customPageIndex; // a custom page index for this menu. Must be >= 22 (the number of original indices)
        [HideInInspector]
        public MenuPage menuPage; // the menu page component of this page

        /*
         * Link to menu page when initializing
         */
        protected virtual void Start()
        {
            CustomMenuPage.instance = this;
            this.menuPage = base.GetComponent<MenuPage>();

            menuPage.menuPageIndex = (MenuPageIndex)this.customPageIndex;
        }
    }
}
