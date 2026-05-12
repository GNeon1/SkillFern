/*
 * SkillNetworkSync.cs
 * 
 * Performs all network operations to synchronize skill data across all players
 */

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using SkillFern.Custom;
using System;

namespace SkillFern.Networking
{
    public static class SkillNetworkSync
    {
        const byte SKILL_DATA_CHANNEL = 187; // photon channel to send skill data on

        /*
         * Initializes the network sync. Run when plugin is awake
         */
        public static void Initialize() {
            // subscribe to photon events
            PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
        }

        public static void OnEventReceived(EventData data) {
            // only continue if the event is a skill update
            if (data.Code != SKILL_DATA_CHANNEL)
                return;

            // deconstruct the payload
            object[] payload = (object[])data.CustomData;
            Plugin.LogInfo("Payload: " + payload.ToString());

            string steamID = (string)payload[0];   // steamID of updated player
            string skillName = (string)payload[1]; // name of skill to update
            int newLevel = (int)payload[2];        // new level to update the skill to

            SkillDataManager.instance.UpdateSkill(steamID, skillName, newLevel);
        }

        /*
         * Updates a given skill to a new value for all players
         * 
         * @param steamID - steamID of the player to update
         * @param skillName - name of the skill to update
         * @param newLevel - new value for the skill level
         */
        public static void UpdateSkill(string steamID, string skillName, int newLevel) {
            // payload containing actual data to send
            object[] payload = new object[] { steamID, skillName, newLevel };

            // message should go to all players
            RaiseEventOptions eventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };

            // send the event on the correct channel with reliable delivery
            PhotonNetwork.RaiseEvent(SKILL_DATA_CHANNEL, payload, eventOptions, SendOptions.SendReliable);
        }

    }
}
