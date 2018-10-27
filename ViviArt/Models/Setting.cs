using SQLite;
using System;

namespace ViviArt
{
    public class Setting
    {
        [NotNull, PrimaryKey]
        public string Key { get; set; }
        public string Value { get; set; }

        public int Save()
        {
            lock (GlobalResources.Current.dbLocker)
            {
                try
                {
                    return GlobalResources.Current.database.Insert(this);
                }
                catch (SQLite.SQLiteException e)
                {
                    return GlobalResources.Current.database.Update(this);
                }
            }
        }
        public static string ValueOf(string key)
        {
            lock (GlobalResources.Current.dbLocker)
            {
                try
                {
                    return GlobalResources.Current.database.Get<Setting>(pk: key).Value;
                }
                catch (System.InvalidOperationException e)
                {
                    return null;
                }
            }
        }
        public static int Save(string key, string value)
        {
            return (new Setting() { Key = key, Value = value }).Save();
        }

        public static Xamarin.Forms.Color GetColor(int corePosition)
        {
            switch(corePosition)
            {
                case 0:
                    return Xamarin.Forms.Color.FromRgb(37, 154, 119);
                case 1:
                    return Xamarin.Forms.Color.FromRgb(236, 125, 48);
                case 2:
                    return Xamarin.Forms.Color.FromRgb(6, 149, 171);
                case 3:
                    return Xamarin.Forms.Color.FromRgb(246, 186, 4);
                case 4:
                    return Xamarin.Forms.Color.FromRgb(135, 190, 184);
                case 5:
                    return Xamarin.Forms.Color.FromRgb(252, 192, 4);
                case 6:
                    return Xamarin.Forms.Color.FromRgb(90, 154, 212);
                case 7:
                    return Xamarin.Forms.Color.FromRgb(219, 112, 88);
                case 8:
                    return Xamarin.Forms.Color.FromRgb(67, 114, 196);

            }
            return Xamarin.Forms.Color.FromRgb(255, 0, 0);
        }

    }
}
