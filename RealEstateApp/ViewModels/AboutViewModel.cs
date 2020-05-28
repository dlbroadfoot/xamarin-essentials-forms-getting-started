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
            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
        }

        public ICommand OpenWebCommand { get; }
        
        // Additional code for Device Display Information clip
        //public override void OnAppearing()
        //{
        //    DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
        //}

        //public override void OnDisappearing()
        //{
        //    DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
        //}

        //private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        //{
        //    OnPropertyChanged(nameof(MainDisplayInfo));
        //}
    }
}