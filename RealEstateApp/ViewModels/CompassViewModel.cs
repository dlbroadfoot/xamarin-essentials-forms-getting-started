using RealEstateApp.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace RealEstateApp.ViewModels
{
    public interface ICompassSensor
    {
        void Start(SensorSpeed sensorSpeed, bool applyLowPassFilter);

        void Stop();

        bool IsMonitoring { get; }

        event EventHandler<CompassChangedEventArgs> ReadingChanged;
    }
    public class CompassSensorImplementation : ICompassSensor
    {
        public void Start(SensorSpeed sensorSpeed, bool applyLowPassFilter) => Compass.Start(sensorSpeed, applyLowPassFilter);

        public void Stop() => Compass.Stop();

        public bool IsMonitoring => Compass.IsMonitoring;

        public event EventHandler<CompassChangedEventArgs> ReadingChanged
        {
            add => Compass.ReadingChanged += value;
            remove => Compass.ReadingChanged -= value;
        }
    }

    public class CompassViewModel : ViewModelBase
    {
        private readonly ICompassSensor _compassSensor;

        public CompassViewModel(ICompassSensor compassSensor)
        {
            _compassSensor = compassSensor;
        }

        public override void OnAppearing()
        {
            _compassSensor.ReadingChanged += OnReadingChanged;

            if (_compassSensor.IsMonitoring == false)
                _compassSensor.Start(SensorSpeed.UI, true);
        }

        public override void OnDisappearing()
        {
            _compassSensor.ReadingChanged -= OnReadingChanged;
            if (_compassSensor.IsMonitoring)
                _compassSensor.Stop();
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
