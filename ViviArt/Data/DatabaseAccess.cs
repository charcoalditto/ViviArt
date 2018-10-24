using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace ViviArt
{
    public class DatabaseAccess
    {
        public static DatabaseAccess Current;
        static DatabaseAccess()
        {
            Current = new DatabaseAccess();
        }

        public T GetItem<T>(int ID) where T : DefaultModel, new()
        {
            lock (GlobalResources.Current.dbLocker)
            {
                try
                {
                    return GlobalResources.Current.database.Get <T>(pk: ID);     
                }
                catch (System.InvalidOperationException e)
                {
                    return null;
                }
            }
        }
        public bool IsExist<T>(int ID) where T : DefaultModel, new()
        {
            lock (GlobalResources.Current.dbLocker)
            {
                var a = GlobalResources.Current.database.Get<T>(pk: ID);
                return true;
            }
        }

        public IEnumerable<T> GetItems<T>() where T : DefaultModel, new()
        {
            lock (GlobalResources.Current.dbLocker)
            {
                return (from i in GlobalResources.Current.database.Table<T>() where i.DeleteDt == null select i).ToList();
            }
        }
        public int CountItems<T>() where T : DefaultModel, new()
        {

            lock (GlobalResources.Current.dbLocker)
            {
                return (from i in GlobalResources.Current.database.Table<T>() where i.DeleteDt == null select i).Count();
            }
        }

        public IEnumerable<TodoItem> GetItemsNotDone()
        {
            lock (GlobalResources.Current.dbLocker)
            {
                return GlobalResources.Current.database.Query<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
            }
        }

        public int SaveItem<T>(T item) where T: DefaultModel
        {
            lock (GlobalResources.Current.dbLocker)
            {
                if (item.ID.HasValue)
                {
                    return GlobalResources.Current.database.Update(item);
                }
                else
                {
                    return GlobalResources.Current.database.Insert(item);
                }
            }
        }
        public int DeleteItem<T>(T item) where T : DefaultModel
        {
            lock (GlobalResources.Current.dbLocker)
            {
                return GlobalResources.Current.database.Delete(item);
            }
        }
    }
}
