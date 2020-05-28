using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using RealEstateApp.ViewModels.Base;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using RealEstateApp.Services.Updates;

namespace RealEstateApp.ViewModels
{
    public class PropertyListViewModel : ViewModelBase
    {
        public ObservableCollection<PropertyListItemViewModel> PropertiesCollection { get; } =
            new ObservableCollection<PropertyListItemViewModel>();

        public IUpdateService UpdateService { get; } = ViewModelLocator.Resolve<IUpdateService>();

        public ICommand PropertySelectedCommand => new Command<PropertyListItemViewModel>(SelectPropertyAsync);
        public ICommand AddPropertyCommand => new Command(AddPropertyAsync);
        public ICommand LoadItemsCommand => new Command(LoadProperties);
        public ICommand SortCommand => new Command(SortAsync);
        private Location _currentLocation;

        private async void SortAsync()
        {
            _currentLocation = await Geolocation.GetLastKnownLocationAsync();
            if (_currentLocation == null)
            {
                _currentLocation = await Geolocation.GetLocationAsync();
            }
            LoadProperties();
        }

        private void LoadProperties()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            PropertiesCollection.Clear();

            try
            {
                var properties = Repository.GetProperties();
                var items = new List<PropertyListItemViewModel>();                

                foreach(var property in properties)
                {
                    var item = new PropertyListItemViewModel(property);
                    if (_currentLocation != null)
                    {
                        item.DistanceInKilometres = _currentLocation.CalculateDistance(property.Latitude, property.Longitude, DistanceUnits.Kilometers);
                    }
                    items.Add(item);
                }

                foreach(var item in items.OrderBy(x=> x.DistanceInKilometres))
                {
                    PropertiesCollection.Add(item);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void AddPropertyAsync()
        {
            await NavigationService.NavigateToModalAsync<AddEditPropertyViewModel>();
        }

        private async void SelectPropertyAsync(PropertyListItemViewModel item)
        {
            await NavigationService.NavigateToAsync<PropertyDetailViewModel>(item.Property);
        }

        public override async Task InitializeAsync(object parameter)
        {
            //UpdateService.CheckForAppUpdatesAsync();

            LoadProperties();

            Repository.ObservePropertySaved()
                .Subscribe(x => LoadProperties());
        }        
    }
}