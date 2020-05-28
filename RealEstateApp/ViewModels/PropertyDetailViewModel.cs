using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using RealEstateApp.Models;
using RealEstateApp.ViewModels.Base;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace RealEstateApp.ViewModels
{
    public class PropertyDetailViewModel : ViewModelBase
    {
        public ICommand EditPropertyCommand => new Command(EditPropertyAsync);
        public ICommand SpeakDescriptionCommand => new Command(SpeakDescriptionAsync);
        public ICommand CancelSpeechCommand => new Command(CancelSpeechAsync);
        public ICommand ViewPhotosCommand => 
            new Command(async () => await NavigationService.NavigateToModalAsync<ImageListViewModel>(Property));
        public ICommand ViewPanoramaCommand => 
            new Command(async () => await NavigationService.NavigateToModalAsync<PanoramaViewModel>(Property));

        public override void OnAppearing()
        {
        }

        public override void OnDisappearing()
        {
        }

        public override async Task InitializeAsync(object parameter)
        {
            Property = (Property)parameter;

            Agent = Repository.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);

            Repository.ObservePropertySaved()
                .Where(x => x.Id == Property.Id)
                .Subscribe(x => Property = x);

            var locales = await TextToSpeech.GetLocalesAsync();
            LocalesCollection = new ObservableCollection<Locale>(locales);
            OnPropertyChanged(nameof(LocalesCollection));
            OnPropertyChanged(nameof(SelectedLocale));
        }

        private CancellationTokenSource _speechCancellation;

        private async void CancelSpeechAsync()
        {
            if (_speechCancellation?.IsCancellationRequested ?? true)
                return;
            _speechCancellation.Cancel();
        }

        private async void SpeakDescriptionAsync()
        {
            _speechCancellation = new CancellationTokenSource();
            IsSpeaking = true;
            var options = new SpeechOptions
            {
                Locale = this.SelectedLocale,
                Volume = this.SelectedVolume,
                Pitch = this.SelectedPitch
            };
            await TextToSpeech.SpeakAsync(Property.Description, options, _speechCancellation.Token);
            IsSpeaking = false;
        }

        public ObservableCollection<Locale> LocalesCollection { get; set; } =
            new ObservableCollection<Locale>();
        
        private float _selectedVolume = 1;

        public float SelectedVolume
        {
            get => _selectedVolume;
            set => SetProperty(ref _selectedVolume, value);
        }

        private float _selectedPitch = 1;

        public float SelectedPitch
        {
            get => _selectedPitch;
            set => SetProperty(ref _selectedPitch, value);
        }

        private Locale _selectedLocale;

        public Locale SelectedLocale
        {
            get => _selectedLocale;
            set => SetProperty(ref _selectedLocale, value);
        }

        private bool _isSpeaking;

        public bool IsSpeaking
        {
            get => _isSpeaking;
            set => SetProperty(ref _isSpeaking, value);
        }

        private Agent _agent;

        public Agent Agent
        {
            get => _agent;
            set => SetProperty(ref _agent, value);
        }

        private Property _property;

        public Property Property
        {
            get => _property;
            set => SetProperty(ref _property, value, forcePropertyChangedNotification: true);
        }

        public ICommand ToggleAdvancedSpeechCommand =>
            new Command(() => ShowAdvancedSpeechControls = !ShowAdvancedSpeechControls);

        private bool _showAdvancedSpeechControls;

        public bool ShowAdvancedSpeechControls
        {
            get => _showAdvancedSpeechControls;
            set => SetProperty(ref _showAdvancedSpeechControls, value);
        }

        private async void EditPropertyAsync()
        {
            await NavigationService.NavigateToModalAsync<AddEditPropertyViewModel>(Property);
        }
    }
}