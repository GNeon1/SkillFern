/*
 * Loads all assets from bundles at launch and serves them as necessary 
 */

using REPOLib;
using SkillFern.UI;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace SkillFern.Utilities
{
    public class AssetHelper
    {

        public static AssetBundle bundle;
        public static bool loaded = false;

        /*
         * Triggers the loading of all bundles from SkillFern.bundle
         */
        public static void LoadBundles() {
            BundleLoader.LoadBundle(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SkillFern.bundle"), OnBundleLoaded, loadContents: true);
        }

        /*
         * Triggers when bundles are loaded
         */
        static IEnumerator OnBundleLoaded(AssetBundle bundle) {
            Plugin.LogInfo("Assets successfully loaded!");

            AssetHelper.bundle = bundle;

            loaded = true;

            yield break;
        }

        /*
         * Registers menus when ready
         */
        public static IEnumerator RegisterMenus(MenuManager menuManager) {
            yield return new WaitUntil(() => loaded);

            // register custom menu pages
            Plugin.LogInfo("Registering menu pages...");

            menuManager.menuPages.Add(new MenuManager.MenuPages()
            {
                menuPageIndex = (MenuPageIndex)MenuPageSkills.CUSTOM_PAGE_INDEX, // cast the custom index to the enum
                menuPage = AssetHelper.GetPrefab(MenuPageSkills.PREFAB_NAME)    // get the prefab for this page
            });

            Plugin.LogInfo("Menu pages registered!");
        }

        /*
         * Returns a prefab by name from the asset bundle
         * @param prefab - name of the prefab to return
         * @returns loaded gameobject
         */
        public static GameObject GetPrefab(string prefab) {
            return bundle.LoadAsset<GameObject>(prefab);
        }
    }
}
