using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Plugin.Toasts;
using Xamarin.Forms;
using DateTimeExtensions;

namespace ViviArt
{
    public class TodoItemEditViewModel : DefaultViewModel<TodoItem>
    {
        public bool NoExpiryDtCheck {
            get
            {
                return MyItem.NoExpiryDt;
            }
            set
            {
                MyItem.NoExpiryDt = value;
                OnPropertyChanged("MyItem");
            }
        }
    }

    public partial class TodoItemEdit : ContentPage
    {
        public TodoItemEditViewModel viewModel { get; set;}
        public delegate void Del();
        public static Del SuccessCallback { get; set; }


        public TodoItemEdit()
        {
            viewModel = new TodoItemEditViewModel();
            viewModel.MyItem.ExpiryDt = DateTime.Now.Date.LastDayOfTheMonth().ToString1();
            viewModel.MyItem.NoExpiryDt = false;
            InitializeComponent();
            BindingContext = viewModel;
        }

        public async void Submit_Clicked(object sender, EventArgs e)
        {
            int res = DatabaseAccess.Current.SaveItem(viewModel.MyItem);
            INotificationResult result = await DependencyService.Get<IToastNotificator>().Notify(new NotificationOptions()
            {
                Title = "저장했습니다",
                Description = $"RES: {res}",
                DelayUntil = DateTime.Now.AddSeconds(1)
                    
            });

            SuccessCallback?.Invoke();
        }
    }
}
