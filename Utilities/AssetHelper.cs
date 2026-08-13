/*
 * Loads all assets from bundles at launch and serves them as necessary 
 */

using REPOLib;
using SkillFern.UI;
using SkillFern.Unity;
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
                menuPage = AssetHelper.GetPrefab(MenuPageSkills.PREFAB_NAME)     // get the prefab for this page
            });

            Plugin.LogInfo("Menu pages registered!");
        }

        /*
         * Registers SkillCubes when ready
         */
        public static IEnumerator RegisterSkillCubes() {
            yield return new WaitUntil(() => loaded);

            Plugin.LogInfo("Waiting to register skill cubes...");

            // find and register skill cubes with ShopHelper
            Plugin.LogInfo("Registering skill cubes...");

            ShopHelper.skillCubes = bundle.LoadAllAssets<SkillCube>();

            // register network prefabs
            foreach (SkillCube cube in ShopHelper.skillCubes)
            {
                cube.maxAmountInShop = 0;
                cube.physicalItem = false;

                StatsManager.instance.itemDictionary.Add(cube.name, cube);

                string targetName = cube.shopPrefab.PrefabName;
                GameObject prefab = GetPrefab(targetName);
                cube.prefab.SetPrefab(prefab, prefab.name);

                REPOLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(prefab);

                Plugin.LogInfo("Registered network prefab for " + prefab.name);
            }

            Plugin.LogInfo(ShopHelper.skillCubes.Length + " skill cubes registered!");
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
