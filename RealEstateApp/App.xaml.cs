using System;
using System.Threading.Tasks;
using RealEstateApp.Services.Dialog;
using RealEstateApp.Services.Navigation;
using RealEstateApp.Services.Updates;
using RealEstateApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace RealEstateApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            InitNavigation();
        }

        protected override async void OnStart()
        {
        }

        private Task InitNavigation()
        {
            var navigationService = ViewModelLocator.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private IDialogService DialogService => ViewModelLocator.Resolve<IDialogService>();
    }
}