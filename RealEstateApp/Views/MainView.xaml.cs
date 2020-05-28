using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace RealEstateApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainView : MasterDetailPage
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void MainView_OnIsPresentedChanged(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                if (IsPresented)
                {
                    var currentPage = Detail;
                    currentPage.FadeTo(0.5);
                }
                else
                {
                    var currentPage = Detail;
                    currentPage.FadeTo(1.0);
                }
            }
        }
    }
}