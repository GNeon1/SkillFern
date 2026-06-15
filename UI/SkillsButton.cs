using UnityEngine;

namespace SkillFern.UI
{
    public class SkillsButton : MonoBehaviour
    {
        /*
         * On click open skills menu
         */
        public void ButtonEventOpenSkills()
        {
            Plugin.LogInfo("Opening skills menu");
            MenuManager.instance.PageSwap((MenuPageIndex)MenuPageSkills.CUSTOM_PAGE_INDEX);
        }
    }
}
