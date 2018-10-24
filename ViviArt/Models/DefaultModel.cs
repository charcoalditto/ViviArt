using SQLite;
using System;
using System.ComponentModel;

namespace ViviArt
{
    public class DefaultModel: INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int? ID { get; set; }
        public string CreateDt { get; set; }
        [Indexed]
        public string DeleteDt { get; set; }

        public DefaultModel()
        {
            CreateDt = DateTime.Now.ToString1();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this,
              new PropertyChangedEventArgs(propertyName));
        }
    }
}
