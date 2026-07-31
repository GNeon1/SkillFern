/*
 * ConfigHelper.cs
 * 
 * Handles configuration implementation and fetching of values
 */

using BepInEx.Configuration;

namespace SkillFern.Utilities
{
    public class ConfigHelper
    {

        private static ConfigEntry<bool> disableShopUpgrades;
        public static bool ShopUpgradesDisabled() { return disableShopUpgrades.Value; }

        private static ConfigEntry<bool> fairDistribution;
        public static bool FairDistribution() { return fairDistribution.Value; }

        private static ConfigEntry<int> startingSkillPoints;
        public static int StartingSkillPoints() { return startingSkillPoints.Value; }

        private static ConfigEntry<int> baseSkillPointsEarned;
        public static int BaseSkillPointsEarned() { return baseSkillPointsEarned.Value; }

        private static ConfigEntry<int> moonSkillPoints;
        public static int MoonSkillPoints() { return moonSkillPoints.Value; }

        private static ConfigEntry<bool> enableDebug;
        public static bool EnableDebug() { return enableDebug == null || enableDebug.Value; }

        /*
         * Initialize all config values
         */
        public static void Initialize(ConfigFile config)
        {
            startingSkillPoints = config.Bind("General", "StartingSkillPoints", 0, new ConfigDescription("The number of skill points to start with (default 0)", new AcceptableValueRange<int>(0, 100)));
            baseSkillPointsEarned = config.Bind("General", "BasePointsEarned", 2, new ConfigDescription("The base number of skill points gained each level (default 2)", new AcceptableValueRange<int>(0, 100)));
            moonSkillPoints = config.Bind("General", "PointsPerMoon", 1, new ConfigDescription("How many additional skill points to earn per moon phase (default 1)", new AcceptableValueRange<int>(0, 100)));

            disableShopUpgrades = config.Bind("Miscellaneous", "DisableUpgrades", true, "Whether to disable the vanilla shop upgrades (on by default)");
            fairDistribution = config.Bind("Miscellaneous", "FairDistribution", true, "Whether to distribute equal skill points to players who joined late or missed levels to ensure they don't fall behind (on by default)");

            enableDebug = config.Bind("Debug", "EnableDebug", false, "Whether to spam the log with my debug info");

        }

    }
}
