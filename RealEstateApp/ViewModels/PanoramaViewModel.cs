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
        }

        public override void OnDisappearing()
        {
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