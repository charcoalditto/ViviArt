using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Plugin.Toasts;
using Xamarin.Forms;

namespace ViviArt
{
    public class MandalaCoreEditViewModel : DefaultViewModel<CoreGoal>
    {
    }
    public partial class MandalaCoreEdit : ContentPage
    {
        public MandalaCoreEditViewModel viewModel { get; set; }

        public delegate void Del();
        public static Del SuccessCallback { get; set; }

        public MandalaCoreEdit()
        {
            InitializeComponent();
            viewModel = new MandalaCoreEditViewModel();
            BindingContext = viewModel;
        }

        public async void Submit_Clicked(object sender, EventArgs e)
        {
            DatabaseAccess.Current.SaveItem(viewModel.MyItem);
            SuccessCallback?.Invoke();
        }
    }
}
