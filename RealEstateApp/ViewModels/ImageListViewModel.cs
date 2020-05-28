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
        }

        public override void OnDisappearing()
        {
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
    }
}