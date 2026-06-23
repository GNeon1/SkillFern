/*
 * MenuPageLobbyPatch.cs
 * 
 * Class for handling all functionality on the lobby menu
 */

using HarmonyLib;
using SkillFern.Custom;
using SkillFern.Networking;
using SkillFern.Utilities;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(MenuPageLobby))]
    public class MenuPageLobbyPatch
    {

        /*
         * AFTER MenuPageLobby.Start
         * 
         * Runs when lobby screen is initialized
         * 
         * Prepares network functionality
         */
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void Start() {
            Plugin.LogInfo("LOBBY PAGE START");

            // initialize network sync and skill data manager
            SkillNetworkSync.Initialize();

            // if not host, initialize SkillDataManager
            if (!PlayerHelper.IsHost())
                SkillDataManager.instance = new SkillDataManager();
        }

        /*
         * BEFORE MenuPageLobbyPatch.ButtonStart
         * 
         * Runs when the game begins by hijacking the click function of the start button
         * 
         * Initializes everything and syncs data
         */
        [HarmonyPatch("ButtonStart")]
        [HarmonyPrefix]
        public static void OnGameStart(bool ___joiningPlayer)
        {
            // if this player is not authorized to start the game, just ignore
            if (___joiningPlayer)
                return;

            // sync all player data
            SkillNetworkSync.SyncAll();
        }

    }
}
