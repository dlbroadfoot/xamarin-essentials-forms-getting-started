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
        public App(System.Drawing.Color? primaryColor = null)
        {
            InitializeComponent();

            if (primaryColor != null)
            {
                Application.Current.Resources["NavigationPrimary"] = primaryColor;
            }

            InitNavigation();
        }

        protected override async void OnStart()
        {
            VersionTracking.Track();

            if (VersionTracking.IsFirstLaunchEver)
            {
                var response = await DialogService.ShowActionSheetAsync("Thanks for installing the app, would you like a tour?", null, null, "Yes", "No");

                if (response == "Yes")
                    Device.OpenUri(new Uri("https://www.youtube.com/watch?v=JaVjmi7MDEs"));
            }
            else if (VersionTracking.IsFirstLaunchForVersion("1.1"))
            {
                await DialogService.ShowAlertAsync(
                    "We've just released a panoramic photo viewer. Check it out on the property details page!",
                    "New Features");
            }

            var updateTask = Task.Run(async () =>
            {
                var updateService = ViewModelLocator.Resolve<IUpdateService>();

                if (await updateService.CheckForAppUpdatesAsync())
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        var mainPage = Current.MainPage;
                        await mainPage.DisplayAlert("App Update", "There is a new version available. Please download it from the App Store", "OK");
                    });
                }

            });
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