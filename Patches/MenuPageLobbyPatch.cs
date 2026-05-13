/*
 * MenuPageLobbyPatch.cs
 * 
 * Class for handling all functionality on the lobby menu
 */

using HarmonyLib;
using SkillFern.Networking;

namespace SkillFern.Patches
{
    [HarmonyPatch(typeof(MenuPageLobby))]
    public class MenuPageLobbyPatch
    {

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
