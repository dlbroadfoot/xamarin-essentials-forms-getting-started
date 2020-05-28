using RealEstateApp.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RealEstateApp.ViewModels
{
    public class HeightCalculatorViewModel : ViewModelBase
    {
        private const double AverageSeaLevelPressure = 1013.25;
        public ObservableCollection<MeasurementViewModel> Measurements { get; set; }
        public ICommand SaveMeasurementCommand => new Command(SaveMeasurement);

        public override async void OnAppearing()
        {
            Barometer.ReadingChanged += OnReadingChanged;

            if (!Barometer.IsMonitoring)
                Barometer.Start(SensorSpeed.UI);
        }

        public override void OnDisappearing()
        {
            Barometer.ReadingChanged -= OnReadingChanged;

            if (Barometer.IsMonitoring)
                Barometer.Stop();
        }

        private void OnReadingChanged(object sender, BarometerChangedEventArgs e)
        {

        }

        private void SaveMeasurement()
        {
           
        }

        private void LoadCalibration()
        {

        }

        private CalibrationItemViewModel _calibrationPressure;

        public CalibrationItemViewModel CalibrationPressure
        {
            get => _calibrationPressure;
            set
            {
                if (SetProperty(ref _calibrationPressure, value))
                    SeaLevelPressure = _calibrationPressure?.Pressure ?? AverageSeaLevelPressure;
            }
        }

        private double _currentPressure;

        public double CurrentPressure
        {
            get => _currentPressure;
            set => SetProperty(ref _currentPressure, value);
        }

        private double _currentAltitude;

        public double CurrentAltitude
        {
            get => _currentAltitude;
            set => SetProperty(ref _currentAltitude, value);
        }

        private double _gpsAltitude;

        public double GPSAltitude
        {
            get => _gpsAltitude;
            set => SetProperty(ref _gpsAltitude, value);
        }
        
        private double _seaLevelPressure = AverageSeaLevelPressure;

        public double SeaLevelPressure
        {
            get => _seaLevelPressure;
            set => SetProperty(ref _seaLevelPressure, value);
        }

        private string _measurementLabel;

        public string MeasurementLabel
        {
            get => _measurementLabel;
            set => SetProperty(ref _measurementLabel, value);
        }

        public ObservableCollection<CalibrationItemViewModel> CalibrationPressures { get; set; }

        public ICommand LoadCalibrationCommand => new Command(LoadCalibration);

        public HeightCalculatorViewModel()
        {
            Measurements = new ObservableCollection<MeasurementViewModel>();
            CalibrationPressures = new ObservableCollection<CalibrationItemViewModel>();
        }

        public class CalibrationItemViewModel
        {
            public double Pressure { get; set; }
            public double Altitude { get; set; }

            public override string ToString()
            {
                return $"{Pressure} hPa ({Altitude:N2}m)";
            }
        }

        public class MeasurementViewModel
        {
            public double Pressure { get; set; }
            public double Altitude { get; set; }
            public string Label { get; set; }
            public double HeightChange { get; set; }

            public string Display => $"{Label}: {Altitude:N2}m";
        }
    }
}
