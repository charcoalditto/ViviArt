using System.ComponentModel;

namespace ViviArt
{
    public class DefaultViewModel<T> : INotifyPropertyChanged where T: DefaultModel, new()
    {
        public int _itemID;
        public int ItemID
        {
            get
            {
                return _itemID;
            }
            set
            {
                _itemID = value;
                if (value == -1)
                {
                    SubmitText = "추가하기";
                    MyItem = new T();
                }
                else
                {
                    SubmitText = "수정하기";
                    MyItem = DatabaseAccess.Current.GetItem<T>(value);
                }
            }
        }
        public T _myItem = new T();
        public T MyItem
        {
            get
            {
                return _myItem;
            }
            set
            {
                _myItem = value;
                OnPropertyChanged("MyItem");
            }
        }
        public string _submitText = "추가하기";
        public string SubmitText
        {
            get
            {
                return _submitText;
            }
            set
            {
                _submitText = value;
                OnPropertyChanged("SubmitText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
