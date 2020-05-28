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


        public ObservableCollection<Agent> Agents { get; }

        public ICommand SaveCommand => new Command(SaveAsync);
        public ICommand CancelCommand => new Command(CancelAsync);
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
        
        public override void OnAppearing()
        {
        }

        public override void OnDisappearing()
        {
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
            NavigationService.PopModalAsync();
        }

        public bool Save()
        {
            if (IsValid() == false)
            {
                StatusMessage = "Please fill in all required fields";
                StatusColor = Color.Red;
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