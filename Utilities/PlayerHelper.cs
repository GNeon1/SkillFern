using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SkillFern.Utilities
{
    public static class PlayerHelper
    {

        const int STARTING_ENERGY = 40; // base energy level for players (TODO: autodetect)
        const int ENERGY_INCREMENT = 10; // amount energy increases per level (TODO: autodetect)

        const int STARTING_HEALTH = 100; // base health level for players (TODO: autodetect)
        public const int HEALTH_INCREMENT = 20; // amount health increases per level (TODO: autodetect)

        const float STARTING_SPEED = 5f; // base speed level for players (TODO: autodetect)

        const float STARTING_STRENGTH = 1.0f; // base strength multiplier for players (TODO: autodetect)
        const float STRENGTH_INCREMENT = 0.2f; // amount strength multiplier increases per level (TODO: autodetect)

        const float STARTING_RANGE = 2.5f; // base grab range for players (TODO: autodetect)

        const float THROW_INCREMENT = 0.3f; // amount throw strength multiplier increases per level (TODO: autodetect)

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
         * @returns whether the local player is host of the lobby
         */
        public static bool IsHost()
        {
            return PhotonNetwork.IsMasterClient;
        }

        /*
         * TODO: description
         */
        public static List<string> GetAllPlayerSteamIDs() {
            List<string> allPlayers = new List<string>();

            foreach (PlayerAvatar avatar in SemiFunc.PlayerGetAll())
                allPlayers.Add((string)AccessTools.Field(typeof(PlayerAvatar), "steamID").GetValue(avatar));

            return allPlayers;
        }

        /*
         * @returns local PlayerController instance
         */
        public static PlayerController GetLocalPlayerController() {
            return PlayerController.instance;
        }

        /*
         * @returns player avatar of the local player
         */
        public static PlayerAvatar GetLocalPlayerAvatar() {
            return PlayerAvatar.instance;
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

            AccessTools.Field(typeof(PlayerController), "EnergyStart").SetValue(playerController, (float)(STARTING_ENERGY + newLevel * ENERGY_INCREMENT));
            AccessTools.Field(typeof(PlayerController), "EnergyCurrent").SetValue(playerController, playerController.EnergyStart);
        }

        /*
         * Sets health to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerHealth(int newLevel, int oldValue)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            int levelDifference = StatsManager.instance.playerUpgradeHealth.ContainsKey(GetLocalSteamID()) ? newLevel - oldValue : newLevel;

            StatsManager.instance.playerUpgradeHealth[GetLocalSteamID()] = newLevel;

            AccessTools.Field(typeof(PlayerHealth), "maxHealth").SetValue(playerAvatar.playerHealth, (int)(STARTING_HEALTH + newLevel * HEALTH_INCREMENT));

            int maxHealth = (int)AccessTools.Field(typeof(PlayerHealth), "maxHealth").GetValue(playerAvatar.playerHealth);
            int currentHealth = (int)AccessTools.Field(typeof(PlayerHealth), "health").GetValue(playerAvatar.playerHealth);

            if (levelDifference != 0)
            {
                CameraGlitch.Instance.PlayShortHeal();
                AccessTools.Field(typeof(PlayerHealth), "health").SetValue(playerAvatar.playerHealth, currentHealth + levelDifference * HEALTH_INCREMENT);
                if (GameManager.Multiplayer())
                {
                    playerAvatar.photonView.RPC("UpdateHealthRPC", RpcTarget.Others, new object[] { maxHealth, currentHealth + levelDifference * HEALTH_INCREMENT, true, false });
                }
            }
        }

        /*
         * Sets extra jump to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerExtraJump(int newLevel)
        {
            PlayerController playerController = GetLocalPlayerController();

            AccessTools.Field(typeof(PlayerController), "JumpExtra").SetValue(playerController, newLevel);
        }

        /*
         * Sets tumble launch to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerLaunch(int newLevel)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            object tumble = AccessTools.Field(typeof(PlayerAvatar), "tumble").GetValue(playerAvatar);

            // only update if tumble object exists
            if (tumble != null)
                AccessTools.Field(typeof(PlayerTumble), "tumbleLaunch").SetValue(tumble, newLevel);
        }

        /*
         * Sets tumble climb to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerTumbleClimb(int newLevel)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            AccessTools.Field(typeof(PlayerAvatar), "upgradeTumbleClimb").SetValue(playerAvatar, (float)newLevel);
        }

        /*
         * Sets death head battery to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerDeathHeadBattery(int newLevel)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            AccessTools.Field(typeof(PlayerAvatar), "upgradeDeathHeadBattery").SetValue(playerAvatar, (float)newLevel);
        }

        /*
         * Sets map player count to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerMapPlayerCount(int newLevel)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            AccessTools.Field(typeof(PlayerAvatar), "upgradeMapPlayerCount").SetValue(playerAvatar, newLevel);
        }

        /*
         * Sets sprint speed to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerSpeed(int newLevel)
        {
            PlayerController playerController = GetLocalPlayerController();

            AccessTools.Field(typeof(PlayerController), "SprintSpeed").SetValue(playerController, STARTING_SPEED + newLevel);
            AccessTools.Field(typeof(PlayerController), "SprintSpeedUpgrades").SetValue(playerController, (float)newLevel);
            AccessTools.Field(typeof(PlayerController), "playerOriginalSprintSpeed").SetValue(playerController, STARTING_SPEED + newLevel);
        }

        /*
         * Sets strength to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerStrength(int newLevel)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            AccessTools.Field(typeof(PhysGrabber), "grabStrength").SetValue(AccessTools.Field(typeof(PlayerAvatar), "physGrabber").GetValue(playerAvatar), STARTING_STRENGTH + (newLevel * STRENGTH_INCREMENT));
        }

        /*
         * Sets grab range to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerRange(int newLevel)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            AccessTools.Field(typeof(PhysGrabber), "grabRange").SetValue(AccessTools.Field(typeof(PlayerAvatar), "physGrabber").GetValue(playerAvatar), STARTING_RANGE + newLevel);
        }

        /*
         * Sets throw strength to given level for the local player (TODO: figure out what this is)
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerThrow(int newLevel)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            AccessTools.Field(typeof(PhysGrabber), "throwStrength").SetValue(AccessTools.Field(typeof(PlayerAvatar), "physGrabber").GetValue(playerAvatar), newLevel * THROW_INCREMENT);
        }

        /*
         * Sets crouch rest to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerCrouchRest(int newLevel)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            AccessTools.Field(typeof(PlayerAvatar), "upgradeCrouchRest").SetValue(playerAvatar, (float)newLevel);
        }

        /*
         * Sets tumble wings to given level for the local player
         * 
         * @param newLevel - new level to set
         */
        public static void UpdatePlayerTumbleWings(int newLevel)
        {
            PlayerAvatar playerAvatar = GetLocalPlayerAvatar();

            AccessTools.Field(typeof(PlayerAvatar), "upgradeTumbleWings").SetValue(playerAvatar, (float)newLevel);
        }

    }
}
