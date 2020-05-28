using RealEstateApp.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace RealEstateApp.ViewModels
{
    public class CompassViewModel : ViewModelBase
    {
        public CompassViewModel()
        {
        }

        public override void OnAppearing()
        {
            Compass.ReadingChanged += OnReadingChanged;

            if (Compass.IsMonitoring == false)
                Compass.Start(SensorSpeed.UI, true);
        }

        public override void OnDisappearing()
        {
            Compass.ReadingChanged -= OnReadingChanged;
            if (Compass.IsMonitoring)
                Compass.Stop();
        }

        private void OnReadingChanged(object sender, CompassChangedEventArgs e)
        {
            CurrentHeading = e.Reading.HeadingMagneticNorth;
            Rotation = CurrentHeading * -1;

            var closest90 = Math.Round(CurrentHeading / 90d, MidpointRounding.AwayFromZero) * 90;
            switch (closest90)
            {
                case 0:
                case 360:
                    CurrentAspect = "North";
                    break;
                case 90:
                    CurrentAspect = "East";
                    break;
                case 180:
                    CurrentAspect = "South";
                    break;
                case 270:
                    CurrentAspect = "West";
                    break;
            }
        }

        private string _currentAspect;

        public string CurrentAspect
        {
            get => _currentAspect;
            set => SetProperty(ref _currentAspect, value);
        }

        private double _currentHeading;

        public double CurrentHeading
        {
            get => _currentHeading;
            set => SetProperty(ref _currentHeading, value);
        }

        private double _rotation;

        public double Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }

        public Task<string> GetAspectAsync()
        {
            _getAspectTask = new TaskCompletionSource<string>();

            return _getAspectTask.Task;
        }

        private TaskCompletionSource<string> _getAspectTask;

        private void Close(bool cancel)
        {
            if (cancel)
            {
                _getAspectTask?.SetResult(null);
            }
            else
            {
                _getAspectTask?.SetResult(CurrentAspect);
            }

            NavigationService.PopModalAsync();
        }

        public ICommand CloseCommand => new Command<bool>(Close);
    }
}
