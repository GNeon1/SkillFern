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

        private static ConfigEntry<int> edgeNodeBase;
        public static int EdgeNodeBase() { return edgeNodeBase.Value; }

        private static ConfigEntry<int> edgeNodeIncrement;
        public static int EdgeNodeIncrement() { return edgeNodeIncrement.Value; }

        private static ConfigEntry<bool> skillPointsPurchaseable;
        public static bool SkillPointsPurchaseable() { return false && skillPointsPurchaseable.Value; }

        private static ConfigEntry<int> skillCubeCost;
        public static float SkillCubeCost() { return (float)skillCubeCost.Value; }

        private static ConfigEntry<int> minShopPointsPerInterval;
        public static int MinShopPointsPerInterval() { return minShopPointsPerInterval.Value; }

        private static ConfigEntry<int> maxShopPointsPerInterval;
        public static int MaxShopPointsPerInterval() { return maxShopPointsPerInterval.Value; }

        private static ConfigEntry<int> levelsPerInterval;
        public static int LevelsPerInterval() { return levelsPerInterval.Value; }

        private static ConfigEntry<int> shopPointsCap;
        public static int ShopPointsCap() { return shopPointsCap.Value; }

        /*
         * Initialize all config values
         */
        public static void Initialize(ConfigFile config)
        {
            startingSkillPoints = config.Bind("General", "StartingSkillPoints", 2, new ConfigDescription("The number of skill points to start with (default 0)", new AcceptableValueRange<int>(0, 100)));
            baseSkillPointsEarned = config.Bind("General", "BasePointsEarned", 1, new ConfigDescription("The base number of skill points gained each level (default 2)", new AcceptableValueRange<int>(0, 100)));
            moonSkillPoints = config.Bind("General", "PointsPerMoon", 1, new ConfigDescription("How many additional skill points to earn per moon phase (default 1)", new AcceptableValueRange<int>(0, 100)));

            edgeNodeBase = config.Bind("Edge Nodes", "EdgeNodeBaseCost", 5, new ConfigDescription("Starting cost for each edge node (default 5)", new AcceptableValueRange<int>(0, 100)));
            edgeNodeIncrement = config.Bind("Edge Nodes", "EdgeNodeIncrement", 1, new ConfigDescription("How much the price of an edge node goes up per purchase (default 1)", new AcceptableValueRange<int>(0, 100)));

            /*skillPointsPurchaseable = config.Bind("Shop", "SkillPointsInShop", true, "Whether skill point cubes should appear in the shop (on by default)");
            skillCubeCost = config.Bind("Shop", "CostPerSkillPoint", 12, new ConfigDescription("How much skill point cubes cost per point (in thousands) (default 10)", new AcceptableValueRange<int>(1, 100)));

            minShopPointsPerInterval = config.Bind("Shop - Advanced", "MinCubePointsPerInterval", 1, new ConfigDescription("Minimum number of skill points purchaseable in shop per interval (default 1)", new AcceptableValueRange<int>(0, 100)));
            maxShopPointsPerInterval = config.Bind("Shop - Advanced", "MaxCubePointsPerInterval", 2, new ConfigDescription("Maximum number of skill points purchaseable in shop per interval (default 2)", new AcceptableValueRange<int>(1, 100)));
            levelsPerInterval = config.Bind("Shop - Advanced", "LevelsPerInterval", 2, new ConfigDescription("Number of levels before interval advances (default 3)", new AcceptableValueRange<int>(1, 100)));
            shopPointsCap = config.Bind("Shop - Advanced", "ShopPointsCap", 12, new ConfigDescription("Max number of points ever available in the shop (default 10)", new AcceptableValueRange<int>(1, 100)));
            */

            disableShopUpgrades = config.Bind("Miscellaneous", "DisableUpgrades", true, "Whether to disable the vanilla shop upgrades (on by default)");
            fairDistribution = config.Bind("Miscellaneous", "FairDistribution", true, "Whether to distribute equal skill points to players who joined late or missed levels to ensure they don't fall behind (on by default)");

            enableDebug = config.Bind("Debug", "EnableDebug", false, "Whether to spam the log with my debug info");

        }

    }
}
