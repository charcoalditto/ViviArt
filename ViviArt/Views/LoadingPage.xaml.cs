using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ViviArt
{
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        public void OnButtonTapped(object sender, EventArgs e)
        {
            DependencyService.Get<ICloseApplication>().Close();
        }
    }
}
