using SQLite;
using System;
using System.ComponentModel;

namespace ViviArt
{
    public class Setting: INotifyPropertyChanged
    {
        public string _key;
        [NotNull, PrimaryKey]
        public string Key 
        { 
            get
            {
                return _key;        
            }
            set
            {
                
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }
        public string _value;
        public string Value 
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this,
              new PropertyChangedEventArgs(propertyName));
        }

        public int Save()
        {
            lock (GlobalResources.Current.dbLocker)
            {
                try
                {
                    return GlobalResources.Current.database.Update(this);
                }
                catch (System.InvalidOperationException e)
                {
                    return GlobalResources.Current.database.Insert(this);
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
        public static void SaveMandalaStaticsUpdateDate(DateTime today)
        {
            var s = new Setting();
            s.Key = SettingKey.MandalaStaticsUpdateDate;
            s.Value = today.ToString2();
            s.Save();
        }
        public static DateTime? GetMandalaStaticsUpdateDate()
        {
            return ValueOf(SettingKey.MandalaStaticsUpdateDate)?.ToDateTime2() ?? null;
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
