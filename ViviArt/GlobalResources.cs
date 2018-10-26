using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

namespace ViviArt
{
	public class GlobalResources
    {
        public static GlobalResources Current;
        public SQLiteConnection database;
        public string dbName;
        public string dbPath;
        public object dbLocker;

        static GlobalResources()
        {
            Current = new GlobalResources();
            Current.dbLocker = new object();
            Current.dbName = "ViviArt.db";
            //DependencyService.Get<IExternalDir>().GetDocumentPath(Current.dbName); 이건 Xamarin.init을 하고 쓰라는 메시지가 뜬다..
            Current.dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), Current.dbName); 
            Current.database = new SQLiteConnection(Current.dbPath);

            Current.InitializeDb();
        }
        public void InitializeDb()
        {
            //database.DropTable<TodoItem>();
            //database.DropTable<CoreGoal>();
            //database.DropTable<MiddleGoal>();
            //database.DropTable<MandalaArtStatistics>();
            //database.DropTable<Setting>();

            database.CreateTable<TodoItem>();
            database.CreateTable<CoreGoal>();
            database.CreateTable<MiddleGoal>();
            database.CreateTable<Setting>();
            database.CreateTable<MandalaArtStatistics>();

            //TestData.InsertTestDataGoal();
            //TestData.InsertTestDataStat();
        }
    }
}
