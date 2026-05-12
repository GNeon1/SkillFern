using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillFern.Utilities
{
    public static class PlayerHelper
    {

        const int STARTING_ENERGY = 40; // base energy level for players (TODO: autodetect)

        /*
         * Checks if a given Steam ID matches the local player. (useful for responding to network events)
         * 
         * @param steamID - Steam ID to check
         */
        public static bool IsLocalSteamID(string steamID)
        {
            return SemiFunc.PlayerAvatarGetFromSteamID(steamID) == SemiFunc.PlayerAvatarLocal();
        }

        /*
         * @returns local PlayerController instance
         */
        public static PlayerController GetLocalPlayerController() {
            return PlayerController.instance;
        }

        /*
         * @returns the Steam ID of the local player
         */
        public static string GetLocalSteamID() {
            return Steamworks.SteamClient.SteamId.Value.ToString();
        }

        /*
         * Sets energy to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerEnergy(int newLevel)
        {
            PlayerController playerController = GetLocalPlayerController();

            StatsManager.instance.playerUpgradeStamina[GetLocalSteamID()] = newLevel;

            AccessTools.Field(typeof(PlayerController), "EnergyStart").SetValue(playerController, (float)(STARTING_ENERGY + newLevel * 10));
            AccessTools.Field(typeof(PlayerController), "EnergyCurrent").SetValue(playerController, playerController.EnergyStart);
        }
    }
}
