using SQLite;
using System;

namespace ViviArt
{
    public class MiddleGoal: DefaultModel
    {
        [NotNull]
        public int CoreGoalID { get; set; }
        [Indexed, NotNull]
        public int Position { get; set; }
        [NotNull]
        public string Title { get; set; }

        public static MiddleGoal GetItem(int coreID, int position)
        {
            var it = (from i in GlobalResources.Current.database.Table<MiddleGoal>() 
                      where i.DeleteDt == null && i.Position == position  && i.CoreGoalID == coreID select i);
            foreach (MiddleGoal c in it)
            {
                return c;
            }
            return null;
        }
    }
}
