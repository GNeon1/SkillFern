/*
 * SkillData.cs
 * 
 * Holds all current skill data of a single player
 */

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
        }

    }
}
