using System;
using System.Windows.Input;
using RealEstateApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace RealEstateApp.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
        }

        public ICommand ResetPreferencesCommand => new Command(ResetPreferencesAsync);
        public ICommand OpenSettingsCommand => new Command(OpenSettings);

        public ICommand OpenAppStoreCommand => new Command(OpenAppStoreAsync);

        private async void OpenAppStoreAsync()
        {
            if (DeviceInfo.DeviceType == DeviceType.Virtual)
            {
                await DialogService.ShowAlertAsync("Rating the app is not supported in the simulator. Please use a physical device instead.", "Not Supported");
                return;
            }
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                Device.OpenUri(new Uri("https://play.google.com/store/apps/details?id=com.pluralsight"));
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                Device.OpenUri(new Uri("https://apps.apple.com/au/app/pluralsight/id431748264"));
            }
        }

        private void OpenSettings()
        {
            AppInfo.ShowSettingsUI();
        }

        private void ResetPreferencesAsync()
        {
            Preferences.Clear("TextToSpeech");
            Preferences.Clear();
        }

        public DisplayInfo MainDisplayInfo => DeviceDisplay.MainDisplayInfo;

        public override void OnAppearing()
        {
            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
        }

        public override void OnDisappearing()
        {
            DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
        }

        private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MainDisplayInfo));
        }
    }
}