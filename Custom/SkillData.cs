/*
 * SkillData.cs
 * 
 * Holds all current skill data of a single player
 */

using Newtonsoft.Json;
using SkillFern.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillFern.Custom
{
    [Serializable]
    public class SkillData
    {

        // enum of all skill types mapped to SKILL_NAMES
        public enum SKILL_TYPE
        {
            HEALTH = 0,
            STAMINA = 1,
            EXTRA_JUMP = 2,
            LAUNCH = 3,
            TUMBLE_CLIMB = 4,
            DEATH_HEAD_BATTERY = 5,
            MAP_PLAYER_COUNT = 6,
            SPEED = 7,
            STRENGTH = 8,
            RANGE = 9,
            THROW = 10,
            CROUCH_REST = 11,
            TUMBLE_WINGS = 12
        }

        public static string[] SKILL_DISPLAY_NAMES =
        {
            "Health",
            "Stamina",
            "Extra Jump",
            "Tumble Launch",
            "Tumble Climb",
            "Death Battery",
            "Player Count",
            "Speed",
            "Strength",
            "Range",
            "Throw",
            "Crouch Rest",
            "Tumble Wings"
        };

        // array of all skill names
        public static string[] SKILL_NAMES = {
            "healthLevels",
            "staminaLevels",
            "extraJumpLevels",
            "launchLevels",
            "tumbleClimbLevels",
            "deathHeadBatteryLevels",
            "mapPlayerCountLevels",
            "speedLevels",
            "strengthLevels",
            "rangeLevels",
            "throwLevels",
            "crouchRestLevels",
            "tumbleWingsLevels"
        };

        public string steamID;
        public int healthLevels;
        public int staminaLevels;
        public int extraJumpLevels;
        public int launchLevels;
        public int tumbleClimbLevels;
        public int deathHeadBatteryLevels;
        public int mapPlayerCountLevels;
        public int speedLevels;
        public int strengthLevels;
        public int rangeLevels;
        public int throwLevels;
        public int crouchRestLevels;
        public int tumbleWingsLevels;

        public int skillPoints;
        public int pointsGained;

        [JsonProperty("ownedNodes")]
        public List<string> ownedNodes { get; set; } // array of all owned skill node IDs

        public SkillData(string steamID) {
            this.steamID = steamID;
            healthLevels = 0;
            staminaLevels = 0;
            extraJumpLevels = 0;
            launchLevels = 0;
            tumbleClimbLevels = 0;
            deathHeadBatteryLevels = 0;
            mapPlayerCountLevels = 0;
            speedLevels = 0;
            strengthLevels = 0;
            rangeLevels = 0;
            throwLevels = 0;
            crouchRestLevels = 0;
            tumbleWingsLevels = 0;
            skillPoints = ConfigHelper.StartingSkillPoints();
            ownedNodes = new List<string>();

            pointsGained = 0;
        }

        [JsonConstructor]
        public SkillData()
        {
            ownedNodes = new List<string>();
        }

    }
}
