/*
 * SaveManager.cs
 * 
 * Manages saving and loading utility with JSON files parallel to normal saves
 */

using SkillFern.Networking;
using System.IO;
using UnityEngine;

namespace SkillFern.Custom
{
    public static class SaveManager
    {

        public static SkillDataManager SkillManager = new SkillDataManager(); // skill manager for current game

        /*
         * Saves current skill data to correct save path
         * 
         * @param fileName - the name of the save to update (no extension)
         */
        public static void Save(string fileName) {
            string directory = Application.persistentDataPath + "/saves/" + fileName; // normal save directory
            string fullPath = directory + "/" + fileName + "_SkillFern.json";         // full path to the custom save data

            // serialize the current skill data into JSON
            string json = JsonUtility.ToJson(SkillManager, true);

            // write data to file and log success
            File.WriteAllText(fullPath, json);
            Plugin.LogInfo($"Saved data to {fullPath}");
        }

        /*
         * Loads save data from given path or creates new skill data if none exists
         * 
         * @param fileName - the name of the save to load from (no extension)
         */
        public static void Load(string fileName) {
            string directory = Application.persistentDataPath + "/saves/" + fileName; // normal save directory
            string fullPath = directory + "/" + fileName + "_SkillFern.json";         // full path to the custom save data

            // if the file already exists. . .
            if (File.Exists(fullPath)) {
                // read in JSON from file
                string json = File.ReadAllText(fullPath);

                // deserialize the data to replace the current data and log success
                SkillManager = JsonUtility.FromJson<SkillDataManager>(json);

                SkillDataManager.instance = SkillManager;

                // load all save data from each skill
                foreach (SkillData skillData in SkillManager.skillDatas) {
                    foreach (string skill in SkillData.SKILL_NAMES) {
                        SkillNetworkSync.UpdateSkill(skillData.steamID, skill, (int)typeof(SkillData).GetField(skill).GetValue(skillData));
                    }
                }

                Plugin.LogInfo($"Loaded save data from {fullPath}");
            } else { // otherwise. . .
                // create new default skill data and log creation
                SkillManager = new SkillDataManager();

                Plugin.LogInfo($"Created new save data at {fullPath}");
            }
        }
    }
}
