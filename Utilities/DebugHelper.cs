/*
 * DebugHelper.cs
 *  
 * Contains a variety of helpful debug functions to call from the C# console in-game
 */

namespace SkillFern.Utilities
{
    public class DebugHelper
    {

        /*
         * Moves to a shop level with re-rolled items
         */
        public static void TestShop() {
            RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Shop);
        }
    }
}
