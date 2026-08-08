/*
 * SkillNetworkSync.cs
 * 
 * Performs all network operations to synchronize skill data across all players
 */

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using SkillFern.Custom;
using SkillFern.Utilities;
using Steamworks;
using System;
using System.Collections.Generic;

namespace SkillFern.Networking
{
    public static class SkillNetworkSync
    {
        const byte SKILL_DATA_CHANNEL = 187; // photon channel to send skill data on
        private static bool isInitialized = false; // whether network sync has been initialized

        public static int saveCountdown = -1; // number of point network events left before a forced save
        public static int careerCountdown = -1; // number of point network events left before career syncing

        enum EVENT_TYPE
        {
            SKILL_UPDATE = 0,
            NODE_PURCHASE = 1,
            POINTS_UPDATE = 2,
            SKILL_SYNC = 3,
            POINTS_SYNC = 4,
            CAREER_SYNC = 5,
            CONFIG_SYNC = 6
        }

        /*
         * Initializes the network sync. Run when game starts
         */
        public static void Initialize() {
            if (isInitialized || PhotonNetwork.NetworkingClient == null)
                return;

            // mark as initialized to prevent multiple initializations
            isInitialized = true;
            saveCountdown = -1;
            careerCountdown = -1;

            // subscribe to photon events
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;

            // log status
            Plugin.LogInfo("Network initialized");
        }

        public static void OnEventReceived(EventData data) {
            // only continue if the event is a skill update
            if (data.Code != SKILL_DATA_CHANNEL)
                return;

            // deconstruct the payload
            object[] payload = (object[])data.CustomData;
            Plugin.LogInfo("Payload: " + payload.ToString());

            string steamID = (string)payload[1];

            switch ((EVENT_TYPE)payload[0])
            {
                case EVENT_TYPE.CONFIG_SYNC:
                    int moonSkillPoints = (int)payload[2]; // amount of skill points to earn per moon phase (for UI)
                    int baseSkillPoints = (int)payload[3]; // amount of skill points to earn per level (for UI)
                    int baseEdgeNode = (int)payload[4]; // base cost of edge nodes
                    int edgeNodeIncrement = (int)payload[5];

                    SkillDataManager.instance.moonSkillPoints = moonSkillPoints;
                    SkillDataManager.instance.baseSkillPoints = baseSkillPoints;
                    SkillDataManager.instance.baseEdgeNode = baseEdgeNode;
                    SkillDataManager.instance.edgeNodeIncrement = edgeNodeIncrement;
                    return;
                case EVENT_TYPE.CAREER_SYNC:
                    
                    break;
                case EVENT_TYPE.NODE_PURCHASE:
                    string nodeID = (string)payload[2];   // ID of the node to purchase
                    bool sync = (bool)payload[3];

                    SkillDataManager.instance.PurchaseNode(steamID, nodeID, sync);
                    break;
                case EVENT_TYPE.SKILL_UPDATE:
                case EVENT_TYPE.SKILL_SYNC:
                    // steamID of updated player
                    string skillName = (string)payload[2]; // name of skill to update
                    int newLevel = (int)payload[3];        // new level to update the skill to

                    SkillDataManager.instance.UpdateSkill(steamID, skillName, newLevel, (EVENT_TYPE)payload[0] == EVENT_TYPE.SKILL_SYNC);
                    break;
                case EVENT_TYPE.POINTS_UPDATE:
                case EVENT_TYPE.POINTS_SYNC:
                    int amount = (int)payload[2];        // amount to update the skill points by

                    SkillDataManager.instance.UpdateSkillPoints(steamID, amount, (EVENT_TYPE)payload[0] == EVENT_TYPE.POINTS_SYNC);

                    // if host, move save countdown
                    if (saveCountdown > 0 && PlayerHelper.IsHost())
                    {
                        saveCountdown--;

                        if (saveCountdown == 0)
                        {
                            saveCountdown = -1;
                            Plugin.LogInfo("Forcing save after skill point updates. . .");
                            SaveManager.Save(SaveManager.lastFile);
                        }
                    }

                    // if host, move career countdown
                    if (careerCountdown > 0 && PlayerHelper.IsHost())
                    {
                        careerCountdown--;
                        if (careerCountdown == 0)
                        {
                            careerCountdown = -1;
                            Plugin.LogInfo("Syncing careers. . .");
                            SyncCareer();
                        }
                    }
                    break;
            }
        }

        /*
         * Updates every player's skill points
         * 
         * @param amount - amount of skill points to award or deduct
         */
        public static void UpdateSkillPointsForAll(int amount) {
            foreach (string steamID in PlayerHelper.GetAllPlayerSteamIDs())
                UpdateSkillPoints(steamID, amount);
        }

        /*
         * Updates a player's skill point count
         * 
         * @param steamID - steamID of the player to update
         * @param amount - amount of skill points to award or deduct
         */
        public static void UpdateSkillPoints(string steamID, int amount, bool sync = false)
        {
            Plugin.LogInfo("   Granting " + amount + " points to " + steamID);

            // if not multiplayer, update locally and return
            if (!GameManager.Multiplayer())
            {
                SkillDataManager.instance.UpdateSkillPoints(steamID, amount);
                return;
            }

            // payload containing actual data to send
            object[] payload = new object[] { sync ? EVENT_TYPE.POINTS_SYNC : EVENT_TYPE.POINTS_UPDATE, steamID, amount };

            // message should go to all players
            RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            // send the event on the correct channel with reliable delivery
            PhotonNetwork.RaiseEvent(SKILL_DATA_CHANNEL, payload, eventOptions, SendOptions.SendReliable);
        }

        /*
         * Updates a given skill to a new value for all players
         * 
         * @param steamID - steamID of the player to update
         * @param skillName - name of the skill to update
         * @param newLevel - new value for the skill level
         */
        public static void UpdateSkill(string steamID, string skillName, int newLevel, bool sync = false)
        {
            // if not multiplayer, update locally and return
            if (!GameManager.Multiplayer())
            {
                SkillDataManager.instance.UpdateSkill(steamID, skillName, newLevel);
                return;
            }

            // payload containing actual data to send
            object[] payload = new object[] { sync ? EVENT_TYPE.SKILL_SYNC : EVENT_TYPE.SKILL_UPDATE, steamID, skillName, newLevel };

            // message should go to all players
            RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            // send the event on the correct channel with reliable delivery
            PhotonNetwork.RaiseEvent(SKILL_DATA_CHANNEL, payload, eventOptions, SendOptions.SendReliable);
        }
        public static void UpdateSkill(string steamID, SkillData.SKILL_TYPE skillType, int newLevel)
        {
            UpdateSkill(steamID, SkillData.SKILL_NAMES[(int)skillType], newLevel);
        }

        /*
         * Purchases a node for a given player for all players
         * 
         * @param steamID - steamID of the player to update
         * @param nodeID - ID of the node to purchase
         */
        public static void PurchaseNode(string steamID, string nodeID, bool sync = false)
        {
            // if not multiplayer, update locally and return
            if (!GameManager.Multiplayer()) {
                SkillDataManager.instance.PurchaseNode(steamID, nodeID);
                return;
            }

            Plugin.LogInfo("Purchasing node " + nodeID + " for " + steamID + " on network.");

            // payload containing actual data to send
            object[] payload = new object[] { EVENT_TYPE.NODE_PURCHASE, steamID, nodeID, sync };

            // message should go to all players
            RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            // send the event on the correct channel with reliable delivery
            PhotonNetwork.RaiseEvent(SKILL_DATA_CHANNEL, payload, eventOptions, SendOptions.SendReliable);
        }

        /*
         * Awards skill points to all players for the completed level
         */
        public static void AwardSkillPointsForLevel()
        {
            // if singleplayer, just update this player
            if (!GameManager.Multiplayer())
            {
                int pointsEarned = SkillDataManager.CalculatePointsEarned();
                SkillDataManager.instance.UpdateSkillPoints(PlayerHelper.GetLocalSteamID(), pointsEarned);
                SaveManager.Save(SaveManager.lastFile);
                return;
            }

            // reset the sync countdown
            saveCountdown = 0;

            // cycle through each player and award points
            foreach (string id in PlayerHelper.GetAllPlayerSteamIDs())
            {
                SkillNetworkSync.saveCountdown += 1;

                int pointsEarned = SkillDataManager.CalculatePointsEarned();
                UpdateSkillPoints(id, pointsEarned);
            }
        }

        /*
         * Syncs every skill between all players
         */
        public static void SyncAll() {
            if (!GameManager.Multiplayer() || !PlayerHelper.IsHost())
                return;

            Plugin.LogInfo("Syncing all skill data. . .");

            List<string> idsSynced = new List<string>();
            List<string> idsInGame = PlayerHelper.GetAllPlayerSteamIDs();

            careerCountdown = 0;

            // for each skill data entry
            foreach (SkillData skillData in SkillDataManager.instance.skillDatas) {
                if (!idsInGame.Contains(skillData.steamID))
                {
                    Plugin.LogInfo("Skipping " + skillData.steamID + " who is not in this game.");
                    continue;
                }

                Plugin.LogInfo("Syncing data for " + skillData.steamID);

                Plugin.LogInfo("Cycling skill datas");
                // for each skill in the entry
                foreach (string skill in SkillData.SKILL_NAMES)
                    // sync that skill
                    UpdateSkill(skillData.steamID, skill, (int)typeof(SkillData).GetField(skill).GetValue(skillData), true);

                Plugin.LogInfo("Cycling owned nodes");
                // for each node purchased
                foreach (string nodeID in skillData.ownedNodes)
                    // sync that node
                    PurchaseNode(skillData.steamID, nodeID, true);

                // sync skill points
                Plugin.LogInfo("Syncing skill points");
                UpdateSkillPoints(skillData.steamID, skillData.skillPoints, true);
                careerCountdown++;

                idsSynced.Add(skillData.steamID);
            }

            int diff = idsInGame.Count - idsSynced.Count;
            if (diff > 0)
                Plugin.LogInfo("Creating new skill data for " + diff + " new players.");

            // cycle through all players without a skill data
            foreach (string id in idsInGame)
            {
                if (idsSynced.Contains(id))
                    continue;

                careerCountdown++;

                // if fair distribution is on, start them with the career points
                if (ConfigHelper.FairDistribution())
                {
                    Plugin.LogInfo("Starting " + id + " with " + SkillDataManager.instance.careerPoints + " points.");
                    UpdateSkillPoints(id, SkillDataManager.instance.careerPoints, true);
                }
                else
                { // otherwise, start them with the host's starting points
                    Plugin.LogInfo("Starting " + id + " with " + ConfigHelper.StartingSkillPoints() + " points.");
                    UpdateSkillPoints(id, ConfigHelper.StartingSkillPoints(), true);
                }

                idsSynced.Add(id);
            }

            Plugin.LogInfo("Skill data synced!");

            Plugin.LogInfo("Syncing moon phase data. . .");

            // sync skill points per moon phase to all players

            // payload containing actual data to send
            object[] payload = new object[] { EVENT_TYPE.CONFIG_SYNC, PlayerHelper.GetLocalSteamID(), ConfigHelper.MoonSkillPoints(), ConfigHelper.BaseSkillPointsEarned(), ConfigHelper.EdgeNodeBase(), ConfigHelper.EdgeNodeIncrement()};

            // message should go to all players
            RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            // send the event on the correct channel with reliable delivery
            PhotonNetwork.RaiseEvent(SKILL_DATA_CHANNEL, payload, eventOptions, SendOptions.SendReliable);

        }

        public static void SyncCareer() {
            Plugin.LogInfo("Career points: " + SkillDataManager.instance.careerPoints);

            // cycle through all synced entries and reconcile career (if enabled)
            if (ConfigHelper.FairDistribution())
            {
                Plugin.LogInfo("Reconciling careers");
                foreach (string id in PlayerHelper.GetAllPlayerSteamIDs())
                {
                    if (id == PlayerHelper.GetLocalSteamID())
                        continue;

                    SkillData skillData = SkillDataManager.instance.GetPlayerData(id);

                    Plugin.LogInfo("Reconciling " + id);
                    Plugin.LogInfo("Points gained: " + skillData.pointsGained);

                    int difference = SkillDataManager.instance.careerPoints - skillData.pointsGained;
                    Plugin.LogInfo("Difference: " + difference);
                    if (difference > 0)
                    {
                        Plugin.LogInfo("Adjusting " + difference + " career points to " + id);
                        UpdateSkillPoints(id, difference);
                    }
                }
            }
        }

    }
}
