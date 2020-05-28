using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using RealEstateApp.ViewModels.Base;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using RealEstateApp.Models;
using System.Numerics;
using Xamarin.Essentials;

namespace RealEstateApp.ViewModels
{
    public class ImageListViewModel : ViewModelBase
    {
        public override void OnAppearing()
        {
            try
            {
                Magnetometer.ReadingChanged += OnMagnetometerReadingChanged;

                if (!Magnetometer.IsMonitoring)
                    Magnetometer.Start(SensorSpeed.UI);

                Accelerometer.ShakeDetected += OnShakeDetected;
                Accelerometer.ReadingChanged += OnAccelerometerReadingChanged;

                if (!Accelerometer.IsMonitoring)
                    Accelerometer.Start(SensorSpeed.UI);

                Gyroscope.ReadingChanged += OnGyroscopeReadingChanged;

                if (!Gyroscope.IsMonitoring)
                    Gyroscope.Start(SensorSpeed.UI);
            }
            catch(FeatureNotSupportedException ex)
            {
                Logger.Debug("Feature not supported: " + ex.Message);
            }            
        }

        public override void OnDisappearing()
        {
            try
            {
                Magnetometer.ReadingChanged -= OnMagnetometerReadingChanged;

                if (Magnetometer.IsMonitoring)
                    Magnetometer.Stop();

                Accelerometer.ShakeDetected -= OnShakeDetected;
                Accelerometer.ReadingChanged -= OnAccelerometerReadingChanged;

                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();

                Gyroscope.ReadingChanged -= OnGyroscopeReadingChanged;

                if (Gyroscope.IsMonitoring)
                    Gyroscope.Stop();
            }
            catch (FeatureNotSupportedException ex)
            {
                Logger.Debug("Feature not supported: " + ex.Message);
            }            
        }

        private bool GyroscopeThresholdExceeded;

        private void OnGyroscopeReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            CurrentAngularVelocity = e.Reading.AngularVelocity;

            var y = e.Reading.AngularVelocity.Y;

            if (y < -2.5)
            {
                if (GyroscopeThresholdExceeded)
                    return;

                GyroscopeThresholdExceeded = true;
                SelectedImageIndex++;
            }
            else if (y > 2.5)
            {
                if (GyroscopeThresholdExceeded)
                    return;

                GyroscopeThresholdExceeded = true;
                SelectedImageIndex--;
            }
            else
            {
                GyroscopeThresholdExceeded = false;
            }
        }

        private bool AccelerometerThresholdExceeded;

        private void OnAccelerometerReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            CurrentAcceleration = e.Reading.Acceleration;

            var x = e.Reading.Acceleration.X;

            if (x < -1.5)
            {
                if (AccelerometerThresholdExceeded)
                    return;

                AccelerometerThresholdExceeded = true;
                SelectedImageIndex--;
            }
            else if (x > 1.5)
            {
                if (AccelerometerThresholdExceeded)
                    return;

                AccelerometerThresholdExceeded = true;
                SelectedImageIndex++;
            }
            else
            {
                AccelerometerThresholdExceeded = false;
            }
        }

        private void OnShakeDetected(object sender, EventArgs e)
        {
            //SelectedImageIndex++;
        }

        private float ReferenceMagneticFieldStrength;
        private bool MagnetometerThresholdExceeded;

        private void OnMagnetometerReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            CurrentMagneticFieldStrength = e.Reading.MagneticField.Length();

            if (ReferenceMagneticFieldStrength == 0)
                ReferenceMagneticFieldStrength = CurrentMagneticFieldStrength;

            var relativeStrength = CurrentMagneticFieldStrength / ReferenceMagneticFieldStrength;

            if (relativeStrength > 10)
            {
                if (MagnetometerThresholdExceeded)
                    return;

                MagnetometerThresholdExceeded = true;
                SelectedImageIndex++;
            }
            else
            {
                MagnetometerThresholdExceeded = false;
            }
        }
        
        public override Task InitializeAsync(object navigationData)
        {
            var property = (Property)navigationData;
            ImageUrls = new ObservableCollection<string>(property.ImageUrls);
            return Task.CompletedTask;
        }

        private ObservableCollection<string> _imageUrls = new ObservableCollection<string>();

        public ObservableCollection<string> ImageUrls
        {
            get => _imageUrls;
            set => SetProperty(ref _imageUrls, value);
        }

        private float _currentMagneticFieldStrength;

        public float CurrentMagneticFieldStrength
        {
            get => _currentMagneticFieldStrength;
            set => SetProperty(ref _currentMagneticFieldStrength, value);
        }

        public ICommand CloseCommand => new Command(async () => await NavigationService.PopModalAsync());

        private int _selectedImageIndex;
        
        public int SelectedImageIndex
        {
            get => _selectedImageIndex;
            set
            {
                var imageCount = ImageUrls.Count;

                if (value >= imageCount)
                    value = 0;
                else if (value < 0)
                    value = imageCount - 1;

                SetProperty(ref _selectedImageIndex, value);
            }
        }
        
        private Vector3 _currentAngularVelocity;

        public Vector3 CurrentAngularVelocity
        {
            get => _currentAngularVelocity;
            set => SetProperty(ref _currentAngularVelocity, value);
        }

        private Vector3 _currentAcceleration;

        public Vector3 CurrentAcceleration
        {
            get => _currentAcceleration;
            set => SetProperty(ref _currentAcceleration, value);
        }
    }
}