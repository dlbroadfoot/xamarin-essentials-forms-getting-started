using System;
using System.Threading.Tasks;
using System.Windows.Input;
using RealEstateApp.ViewModels.Base;
using Xamarin.Forms;
using RealEstateApp.Models;
using System.Numerics;
using Xamarin.Essentials;

namespace RealEstateApp.ViewModels
{
    public class PanoramaViewModel : ViewModelBase
    {
        public override void OnAppearing()
        {
            try
            {
                OrientationSensor.ReadingChanged += OnOrientationReadingChanged;

                if (!OrientationSensor.IsMonitoring)
                    OrientationSensor.Start(SensorSpeed.UI);
            }
            catch (FeatureNotSupportedException ex)
            {
                Logger.Debug("Feature not supported: " + ex.Message);
            }
        }

        public override void OnDisappearing()
        {
            try
            {
                OrientationSensor.ReadingChanged -= OnOrientationReadingChanged;

                if (OrientationSensor.IsMonitoring)
                    OrientationSensor.Stop();
            }
            catch (FeatureNotSupportedException ex)
            {
                Logger.Debug("Feature not supported: " + ex.Message);
            }
        }

        private Quaternion _origin = Quaternion.Identity;

        private void OnOrientationReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            if (Orientation == e.Reading.Orientation)
                return;

            if (_origin == Quaternion.Identity)
                _origin = Quaternion.Inverse(e.Reading.Orientation);

            Orientation = Quaternion.Multiply(_origin, e.Reading.Orientation);

            CalculateEulerAngles(Orientation, out var roll, out var yaw, out var pitch);

            TranslationX = (yaw / Math.PI) * PanoramaWidth;
            TranslationY = (roll / Math.PI) * PanoramaHeight;
        }

        private void CalculateEulerAngles(Quaternion q, out double roll, out double yaw, out double pitch)
        {
            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            double unit = sqx + sqy + sqz + sqw;  // if normalised quaternion, the unit is one, otherwise is correction factor
            double test = q.X * q.Y + q.Z * q.W;
            if (test > 0.499 * unit) // singularity at north pole
            {
                yaw = 2 * Math.Atan2(q.X, q.W);
                pitch = Math.PI / 2;
                roll = 0;
            }
            else if (test < -0.499 * unit) // singularity at south pole
            {
                yaw = -2 * Math.Atan2(q.X, q.W);
                pitch = -Math.PI / 2;
                roll = 0;
            }
            else
            {
                yaw = Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, sqx - sqy - sqz + sqw);
                pitch = Math.Asin(2 * test / unit);
                roll = Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, -sqx + sqy - sqz + sqw);
            }
        }

        private Quaternion _orientation;

        public Quaternion Orientation
        {
            get => _orientation;
            set => SetProperty(ref _orientation, value);
        }

        public ICommand CloseCommand => new Command(() => NavigationService.PopModalAsync());

        public override Task InitializeAsync(object navigationData)
        {
            var property = (Property)navigationData;
            PanoramaUrl = property.PanoramaImage.Url;
            PanoramaHeight = property.PanoramaImage.Height;
            PanoramaWidth = property.PanoramaImage.Width;
            return Task.CompletedTask;
        }

        private string _panoramaUrl;

        public string PanoramaUrl
        {
            get => _panoramaUrl;
            set => SetProperty(ref _panoramaUrl, value);
        }

        private int _panoramaWidth;

        public int PanoramaWidth
        {
            get => _panoramaWidth;
            set => SetProperty(ref _panoramaWidth, value);
        }

        private int _panoramaHeight;

        public int PanoramaHeight
        {
            get => _panoramaHeight;
            set => SetProperty(ref _panoramaHeight, value);
        }

        private double _translationX;

        public double TranslationX
        {
            get => _translationX;
            set => SetProperty(ref _translationX, value);
        }

        private double _translationY;

        public double TranslationY
        {
            get => _translationY;
            set => SetProperty(ref _translationY, value);
        }
    }
}