/*
 * SkillDataManager.cs
 * 
 * Holds and manages all current skill and fern data of all players
 */

using HarmonyLib;
using Newtonsoft.Json;
using SkillFern.Networking;
using SkillFern.UI;
using SkillFern.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillFern.Custom
{
    [Serializable]
    public class SkillDataManager
    {
        public static SkillDataManager instance;

        [JsonProperty("skillDatas")]
        public List<SkillData> skillDatas { get; set; } // list of skill data objects for each player

        public int careerPoints;    // total number of skill points earned this run
        public int moonSkillPoints; // how many skill points to earn per moon phase (for UI only)
        public int baseSkillPoints; // how many skill points to earn per level (for UI only)
        public int baseEdgeNode;
        public int edgeNodeIncrement;

        /*
         * Default constructor intializes variables (CALLED BY NEWTONSOFT. DO NOT INITIALIZE INSTANCE)
         */
        [JsonConstructor]
        public SkillDataManager()
        {
            Plugin.LogInfo("NEW SKILLDATAMANAGER CREATED ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            // create an empty list for skill data
            skillDatas = new List<SkillData>();
            careerPoints = ConfigHelper.StartingSkillPoints();
            moonSkillPoints = ConfigHelper.MoonSkillPoints();
            baseSkillPoints = ConfigHelper.BaseSkillPointsEarned();
            baseEdgeNode = ConfigHelper.EdgeNodeBase();
            edgeNodeIncrement = ConfigHelper.EdgeNodeIncrement();
        }

        public static void Initialize()
        {
            if (SkillDataManager.instance == null)
                SkillDataManager.instance = new SkillDataManager();
        }

        /* @param offset - how many levels to offset the calculation by
         * 
         * @returns how many skill points each player earns for this level
         */
        public static int CalculatePointsEarned(int offset = 0) {
            return SkillDataManager.instance.baseSkillPoints + RunManager.instance.CalculateMoonLevel(SemiFunc.RunGetLevelsCompleted() - 1 + offset) * SkillDataManager.instance.moonSkillPoints;
        }

        /* @returns number of skill points held by local player
         */
        public int GetLocalSkillPoints() {
            return GetPlayerData(PlayerHelper.GetLocalSteamID()).skillPoints;
        }

        /* @returns local player's data
         */
        public SkillData GetLocalPlayerData()
        {
            return GetPlayerData(PlayerHelper.GetLocalSteamID());
        }

        /*
         * Returns the skill data object for a given player
         * @param steamID - the steamID of the player to get data for
         */
        public SkillData GetPlayerData(string steamID)
        {
            // find the skill data object that has the given steamID
            var playerData = skillDatas.Find(data => data.steamID == steamID);

            // if the player does not exist. . .
            if (playerData == null)
            {
                // create new skill data for the player
                playerData = new SkillData(steamID);
                skillDatas.Add(playerData);
            }

            // return the found or created skill data
            return playerData;
        }

        /*
         * Updates the skill points of a given player incrementally
         * 
         * @param steamID - Steam ID of the player to update
         * @param amount - number of skill points to add or remove
         */
        public void UpdateSkillPoints(string steamID, int amount, bool syncing = false) {
            // repair skill datas if broken
            if (skillDatas == null)
            {
                Plugin.LogInfo("Skill Data does not exist! Creating. . .");
                skillDatas = new List<SkillData>();
            }

            // find the skill data object for the given player
            SkillData playerData = skillDatas.Find(data => data.steamID == steamID);

            // if the player does not exist, create new skill data for them
            if (playerData == null)
            {
                // create new skill data for the player
                playerData = new SkillData(steamID);
                skillDatas.Add(playerData);

                // if syncing, update points gained
                if (syncing)
                    playerData.pointsGained = amount;

                Plugin.LogInfo("!!! CREATED NEW SKILL DATA with " + playerData.pointsGained + " points");
            }

            // if freshly syncing points, just set the amount
            if (syncing)
                playerData.skillPoints = amount;
            // otherwise. . .
            else
            {
                // if not subtracting. . .
                if (amount > 0)
                {
                    // update career points as self
                    if (steamID == PlayerHelper.GetLocalSteamID())
                        careerPoints += amount;

                    // increase points gained for specified player
                    playerData.pointsGained += amount;
                }

                playerData.skillPoints += amount;
            }

            if (steamID == PlayerHelper.GetLocalSteamID())
            {
                CameraGlitch.Instance.PlayUpgrade();
                if (MenuPageSkills.instance != null)
                    MenuPageSkills.instance.UpdateSkillPointsText();
            }

            Plugin.LogInfo(steamID + " now has " + playerData.skillPoints + " points");
        }

        /*
         * Updates the level of a given skill for a specific player incrementally
         * 
         * @param steamID - Steam ID of the player to update
         * @param skillName - name of the skill to update (must match variable name in SkillData)
         * @param newLevel - new level to update the skill to
         * @param syncing - whether this update is for the initial sync at the round start (skips actually providing stats)
         */
        public void UpdateSkill(string steamID, string skillName, int newLevel, bool syncing = false)
        {

            // repair skill datas if broken
            if (skillDatas == null)
            {
                Plugin.LogInfo("Skill Data does not exist! Creating. . .");
                skillDatas = new List<SkillData>();
            }

            // find the skill data object for the given player
            SkillData playerData = skillDatas.Find(data => data.steamID == steamID);

            // if the player does not exist, create new skill data for them
            if (playerData == null)
            {
                // create new skill data for the player
                playerData = new SkillData(steamID);
                skillDatas.Add(playerData);
            }

            // calculate the new level to set to
            int finalLevel;
            if (syncing)
                finalLevel = newLevel;
            else
                finalLevel = (int)typeof(SkillData).GetField(skillName).GetValue(playerData) + newLevel;

            // update the specified skill to the new value
            typeof(SkillData).GetField(skillName).SetValue(playerData, finalLevel);

            // if this is a syncing update, stop here
            if (syncing)
                return;

            // update the skill in the dictionary for the given player
            int oldValue = UpdateDictionarySkillByName(steamID, skillName, finalLevel);

            // if the update is for the local player or on singleplayer, update their skill accordingly
            if (!GameManager.Multiplayer() || PlayerHelper.IsLocalSteamID(steamID))
                UpdateLocalSkillByName(skillName, finalLevel, oldValue);

        }

        /*
         * Purchases a skill node for a given player
         * 
         * @param steamID - Steam ID of the player to update
         * @param nodeID - ID of the node to purchase
         */
        public void PurchaseNode(string steamID, string nodeID, bool sync = false)
        {
            // repair skill datas if broken
            if (skillDatas == null)
            {
                Plugin.LogInfo("Skill Data does not exist! Creating. . .");
                skillDatas = new List<SkillData>();
            }

            // find the skill data object for the given player
            SkillData playerData = skillDatas.Find(data => data.steamID == steamID);

            // if the player does not exist, create new skill data for them
            if (playerData == null)
            {
                // create new skill data for the player
                Plugin.LogInfo("Creating skill data for player " + steamID);
                playerData = new SkillData(steamID);
                skillDatas.Add(playerData);
            }

            // add the new node to the player's list of owned nodes
            if (!(sync && PlayerHelper.IsHost()))
                playerData.ownedNodes.Add(nodeID);
            else
                Plugin.LogInfo("SKIPPING NODE ADDING");
        }

        /*
         * Updates the level of a skill on the local player based on its variable name (does not network)
         * 
         * @param skillName - skill to update
         * @param newLevel - new level to update the skill to
         */
        public void UpdateLocalSkillByName(string skillName, int newLevel, int oldValue = 0)
        {
            switch (skillName)
            {
                case "healthLevels":
                    PlayerHelper.UpdatePlayerHealth(newLevel, oldValue);
                    break;
                case "staminaLevels":
                    PlayerHelper.UpdatePlayerEnergy(newLevel);
                    break;
                case "extraJumpLevels":
                    PlayerHelper.UpdatePlayerExtraJump(newLevel);
                    break;
                case "launchLevels":
                    PlayerHelper.UpdatePlayerLaunch(newLevel);
                    break;
                case "tumbleClimbLevels":
                    PlayerHelper.UpdatePlayerTumbleClimb(newLevel);
                    break;
                case "deathHeadBatteryLevels":
                    PlayerHelper.UpdatePlayerDeathHeadBattery(newLevel);
                    break;
                case "mapPlayerCountLevels":
                    PlayerHelper.UpdatePlayerMapPlayerCount(newLevel);
                    break;
                case "speedLevels":
                    PlayerHelper.UpdatePlayerSpeed(newLevel);
                    break;
                case "strengthLevels":
                    PlayerHelper.UpdatePlayerStrength(newLevel);
                    break;
                case "rangeLevels":
                    PlayerHelper.UpdatePlayerRange(newLevel);
                    break;
                case "throwLevels":
                    PlayerHelper.UpdatePlayerThrow(newLevel);
                    break;
                case "crouchRestLevels":
                    PlayerHelper.UpdatePlayerCrouchRest(newLevel);
                    break;
                case "tumbleWingsLevels":
                    PlayerHelper.UpdatePlayerTumbleWings(newLevel);
                    break;
            }
        }

        /*
         * Updates the level of a skill on a given player in the dictionary based on its variable name (does not network)
         * 
         * @param steamID - steam ID of the player to update
         * @param skillName - skill to update
         * @param newLevel - new level to update the skill to
         * 
         * @returns old value of that skill if necessary
         */
        public int UpdateDictionarySkillByName(string steamID, string skillName, int newLevel)
        {
            int oldValue = 0;
            switch (skillName)
            {
                case "healthLevels":
                    // find the old value of health for this player
                    oldValue = StatsManager.instance.playerUpgradeHealth.ContainsKey(steamID) ? StatsManager.instance.playerUpgradeHealth[steamID] : 0;

                    // find the player's current health and update it based on the level difference
                    int currentHealth = StatsManager.instance.GetPlayerHealth(steamID);
                    Plugin.LogInfo("Updating health starting at " + currentHealth);
                    StatsManager.instance.SetPlayerHealth(steamID, currentHealth + (newLevel - oldValue) * PlayerHelper.HEALTH_INCREMENT, true);

                    StatsManager.instance.playerUpgradeHealth[steamID] = newLevel;
                    break;
                case "staminaLevels":
                    StatsManager.instance.playerUpgradeStamina[steamID] = newLevel;
                    break;
                case "extraJumpLevels":
                    StatsManager.instance.playerUpgradeExtraJump[steamID] = newLevel;
                    break;
                case "launchLevels":
                    StatsManager.instance.playerUpgradeLaunch[steamID] = newLevel;
                    break;
                case "tumbleClimbLevels":
                    StatsManager.instance.playerUpgradeTumbleClimb[steamID] = newLevel;
                    break;
                case "deathHeadBatteryLevels":
                    StatsManager.instance.playerUpgradeDeathHeadBattery[steamID] = newLevel;
                    break;
                case "mapPlayerCountLevels":
                    StatsManager.instance.playerUpgradeMapPlayerCount[steamID] = newLevel;
                    break;
                case "speedLevels":
                    StatsManager.instance.playerUpgradeSpeed[steamID] = newLevel;
                    break;
                case "strengthLevels":
                    StatsManager.instance.playerUpgradeStrength[steamID] = newLevel;
                    break;
                case "rangeLevels":
                    StatsManager.instance.playerUpgradeRange[steamID] = newLevel;
                    break;
                case "throwLevels":
                    StatsManager.instance.playerUpgradeThrow[steamID] = newLevel;
                    break;
                case "crouchRestLevels":
                    StatsManager.instance.playerUpgradeCrouchRest[steamID] = newLevel;
                    break;
                case "tumbleWingsLevels":
                    StatsManager.instance.playerUpgradeTumbleWings[steamID] = newLevel;
                    break;
            }
            return oldValue;
        }

    }
}
