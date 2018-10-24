using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Plugin.Toasts;
using Xamarin.Forms;

namespace ViviArt
{
    public class MandalaMiddleEditViewModel : DefaultViewModel<MiddleGoal>
    {
    }
    public partial class MandalaMiddleEdit : ContentPage
    {
        public MandalaMiddleEditViewModel viewModel { get; set; }

        public delegate void Del();
        public static Del SuccessCallback { get; set; }

        public MandalaMiddleEdit()
        {
            InitializeComponent();
            viewModel = new MandalaMiddleEditViewModel();
            BindingContext = viewModel;
        }

        public async void Submit_Clicked(object sender, EventArgs e)
        {
            DatabaseAccess.Current.SaveItem(viewModel.MyItem);
            SuccessCallback?.Invoke();
        }
    }
}
