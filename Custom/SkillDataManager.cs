/*
 * SkillDataManager.cs
 * 
 * Holds and manages all current skill and fern data of all players
 */

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

        public List<SkillData> skillDatas; // list of skill data objects for each player

        /*
         * Default constructor intializes static instance of the manager (only one should ever exist)
         */
        public SkillDataManager()
        {
            // instantiate all variables
            skillDatas = new List<SkillData>();
            instance = this;
        }

        public static void Initialize()
        {
            if (SkillDataManager.instance == null)
                SkillDataManager.instance = new SkillDataManager();
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
         * Updates the level of a given skill for a specific player
         * 
         * @param steamID - Steam ID of the player to update
         * @param skillName - name of the skill to update (must match variable name in SkillData)
         * @param newLevel - new level to update the skill to
         */
        public void UpdateSkill(string steamID, string skillName, int newLevel) {

            // repair skill datas if broken
            if (skillDatas == null) {
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

            // update the specified skill to the new value
            typeof(SkillData).GetField(skillName).SetValue(playerData, newLevel);

            // if the update is for the local player, update their skill accordingly
            if (PlayerHelper.IsLocalSteamID(steamID))
                UpdateLocalSkillByName(skillName, newLevel);
        }

        /*
         * Updates the level of a skill on the local player based on its variable name (does not network)
         * 
         * @param skillName - skill to update
         * @param newLevel - new level to update the skill to
         */
        public void UpdateLocalSkillByName(string skillName, int newLevel) {
            switch (skillName)
            {
                case "healthLevels":
                    PlayerHelper.UpdatePlayerHealth(newLevel);
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

    }
}
