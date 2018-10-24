using SQLite;

namespace ViviArt
{
    public class CoreGoal: DefaultModel
    {
        [Indexed, NotNull]
        public int Position { get; set; }
        [NotNull]
        public string Title { get; set; }

        public static CoreGoal GetItem(int position)
        {
            var it = (from i in GlobalResources.Current.database.Table<CoreGoal>() 
                      where i.DeleteDt == null && i.Position == position select i);
            foreach (CoreGoal c in it)
            {
                return c;
            }
            return null;
        }
    }
}
