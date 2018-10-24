using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
namespace ViviArt
{
	public class GlobalResources
    {
        public static GlobalResources Current;
		public SQLiteConnection database;
        public string dbName;
        public string path;
        public object dbLocker;

        static GlobalResources()
        {
            Current = new GlobalResources();
            Current.dbName = "ViviArt.db";
            Current.path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            Current.database = new SQLiteConnection(System.IO.Path.Combine(Current.path, Current.dbName));
            Current.dbLocker = new object();
            Current.InitializeDb();
        }
        public void InitializeDb()
        {
            //database.DropTable<TodoItem>();
            //database.DropTable<CoreGoal>();
            //database.DropTable<MiddleGoal>();
            //database.DropTable<MandalaArtStatistics>();

            database.CreateTable<TodoItem>();
            database.CreateTable<CoreGoal>();
            database.CreateTable<MiddleGoal>();
            database.CreateTable<MandalaArtStatistics>();

            //TestData.InsertTestDataGoal();
            //TestData.InsertTestDataStat();
        }
    }
}
