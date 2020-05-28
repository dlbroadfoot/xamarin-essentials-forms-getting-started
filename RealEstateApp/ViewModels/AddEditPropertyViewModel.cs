using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using RealEstateApp.Models;
using RealEstateApp.ViewModels.Base;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RealEstateApp.ViewModels
{
    public class AddEditPropertyViewModel : ViewModelBase
    {
        public AddEditPropertyViewModel()
        {
            Property = new Property();
            Agents = new ObservableCollection<Agent>(Repository.GetAgents());
        }

        public async Task GetLocationAsync()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best);
            var location = await Geolocation.GetLocationAsync(request);

            Property.Latitude = location.Latitude;
            Property.Longitude = location.Longitude;

            var addresses = await Geocoding.GetPlacemarksAsync(location);
            var address = addresses.FirstOrDefault();
            if (address != null)
            {
                Property.Address = $"{address.SubThoroughfare} {address.Thoroughfare}, {address.Locality} {address.AdminArea} {address.PostalCode} {address.CountryName}";
            }

            OnPropertyChanged(nameof(Property));
        }


        public ObservableCollection<Agent> Agents { get; }

        public ICommand SaveCommand => new Command(SaveAsync);
        public ICommand CancelCommand => new Command(CancelAsync);
        public ICommand GetLocationCommand => new Command(async () => await GetLocationAsync());
        public ICommand GeocodeCommand => new Command(GeocodeAsync);
        public bool IsOnline => Xamarin.Essentials.Connectivity.NetworkAccess == NetworkAccess.Internet;
        public ICommand GetCurrentAspectCommand => new Command(GetCurrentAspectAsync);

        private async void GetCurrentAspectAsync()
        {
            var modal = await NavigationService.NavigateToModalAsync<CompassViewModel>();
            var currentAspect = await modal.GetAspectAsync();
            if (currentAspect != null)
            {
                Property.Aspect = currentAspect;
                OnPropertyChanged(nameof(Property));
            }
        }

        private void CheckBatteryStatus()
        {
            if (Battery.ChargeLevel < 0.2)
            {
                StatusMessage = "Low battery. Please save changes asap";
                if (Battery.State != BatteryState.Charging)
                    StatusColor = System.Drawing.Color.Red;
                else
                    StatusColor = System.Drawing.Color.Yellow;

                if (Battery.EnergySaverStatus == EnergySaverStatus.On)
                    StatusColor = System.Drawing.Color.Green;
            }
            else
            {
                StatusMessage = null;
            }
        }
        
        public override void OnAppearing()
        {
            Connectivity.ConnectivityChanged += OnConnectivityChanged;
            CheckBatteryStatus();
            Battery.BatteryInfoChanged += OnBatteryInfoChanged;
            Battery.EnergySaverStatusChanged += OnEnergySaverStatusChanged;
        }

        private void OnEnergySaverStatusChanged(object sender, EnergySaverStatusChangedEventArgs e)
        {
            CheckBatteryStatus();
        }

        private void OnBatteryInfoChanged(object sender, BatteryInfoChangedEventArgs e)
        {
            CheckBatteryStatus();
        }

        public override void OnDisappearing()
        {
            Connectivity.ConnectivityChanged -= OnConnectivityChanged;
            Battery.BatteryInfoChanged -= OnBatteryInfoChanged;
            Battery.EnergySaverStatusChanged -= OnEnergySaverStatusChanged;
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            Logger.Debug(e.NetworkAccess);
            Logger.Debug(e.ConnectionProfiles);
            OnPropertyChanged(nameof(IsOnline));
        }

        private async void GeocodeAsync()
        {
            if (string.IsNullOrWhiteSpace(Property.Address))
            {
                await DialogService.ShowAlertAsync("Please enter an address", "No Address");
                return;
            }
            if (IsOnline == false)
            {
                await DialogService.ShowAlertAsync("You must be online to use geocoding", "Offline");
                return;
            }

            var locations = await Geocoding.GetLocationsAsync(Property.Address);
            var location = locations.FirstOrDefault();
            if (location != null)
            {
                Property.Latitude = location.Latitude;
                Property.Longitude = location.Longitude;
                OnPropertyChanged(nameof(Property));
            }
        }

        private Property _property;

        public Property Property
        {
            get => _property;
            set
            {
                SetProperty(ref _property, value);
                SelectedAgent = Agents?.FirstOrDefault(x => x.Id == _property?.AgentId);
            }
        }

        private Agent _selectedAgent;

        public Agent SelectedAgent
        {
            get => _selectedAgent;
            set
            {
                if (!SetProperty(ref _selectedAgent, value)) return;

                if (Property != null) Property.AgentId = _selectedAgent?.Id;
            }
        }

        private string _statusMessage;

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private Color _statusColor = Color.White;

        public Color StatusColor
        {
            get => _statusColor;
            set => SetProperty(ref _statusColor, value);
        }

        public override async Task InitializeAsync(object navigationData)
        {
            Property = navigationData as Property;

            if (Property == null)
            {
                Title = "Add Property";
                Property = new Property();

                var data = await Clipboard.GetTextAsync();

                if (data?.Contains("\"Address\":") == true)
                {
                    Property = JsonConvert.DeserializeObject<Property>(data);
                }
            }
            else
            {
                Title = "Edit Property";
            }
        }

        private void SaveAsync()
        {
            if (Save()) NavigationService.PopModalAsync();
        }

        private void CancelAsync()
        {
            Vibration.Cancel();
            NavigationService.PopModalAsync();
        }

        public bool Save()
        {
            if (IsValid() == false)
            {
                StatusMessage = "Please fill in all required fields";
                StatusColor = Color.Red;
                Xamarin.Essentials.Vibration.Cancel();
                Vibration.Vibrate(5000);
                return false;
            }

            Repository.SaveProperty(Property);

            return true;
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Property.Address)
                || Property.Beds == null
                || Property.Price == null
                || Property.AgentId == null)
                return false;

            return true;
        }
    }
}