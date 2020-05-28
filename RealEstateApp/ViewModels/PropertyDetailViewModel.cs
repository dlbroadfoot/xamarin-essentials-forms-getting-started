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

        public ICommand PhoneCommand => new Command<Vendor>(UsePhoneAsync);
        public ICommand SendEmailCommand => new Command<Vendor>(SendEmailAsync);
        public ICommand OpenMapsCommand => new Command<NavigationMode>(OpenMapsAsync);
        public ICommand OpenBrowserCommand => new Command<BrowserLaunchMode>(OpenBrowserAsync);
        public ICommand OpenUberCommand { get; set; }
        public ICommand OpenFileCommand => new Command(OpenFileAsync);
        public ICommand ShareTextCommand => new Command(ShareTextAsync);
        public ICommand ShareFileCommand => new Command(ShareFileAsync);
        public ICommand CopyToClipboardCommand => new Command(CopyToClipboardAsync);

        public override void OnAppearing()
        {
            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
        }

        private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            if (e.DisplayInfo.Orientation == DisplayOrientation.Landscape)
            {
                DeviceDisplay.KeepScreenOn = true;
                ViewPhotosCommand.Execute(null);
            }
        }

        public override void OnDisappearing()
        {
            DeviceDisplay.KeepScreenOn = false;
            DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
        }

        private async void CopyToClipboardAsync()
        {
            var data = JsonConvert.SerializeObject(Property);

            await Clipboard.SetTextAsync(data);
        }

        private async void ShareFileAsync()
        {
            ExperimentalFeatures.Enable(ExperimentalFeatures.ShareFileRequest, ExperimentalFeatures.OpenFileRequest);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share Property Contract",
                File = new ShareFile(Property.ContractFilePath)
            });
        }

        private async void ShareTextAsync()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = Property.NeighbourhoodUrl,
                Subject = "A property you may be interested in",
                Text = $"{Property.Address} - {Property.Price:C0} - {Property.Beds} beds",
                Title = "Share Property"
            });
        }

        private async void OpenFileAsync()
        {
            var filePath = Property.ContractFilePath;

            ExperimentalFeatures.Enable(ExperimentalFeatures.OpenFileRequest, ExperimentalFeatures.ShareFileRequest);

            await Launcher.OpenAsync(
                new OpenFileRequest("Contract",
                  new ReadOnlyFile(filePath)));
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

            if (await Launcher.CanOpenAsync("uber://"))
            {
                OpenUberCommand = new Command(OpenUberAsync);
                OnPropertyChanged(nameof(OpenUberCommand));
            }
        }

        private async void OpenUberAsync()
        {
            await Launcher.OpenAsync($"uber://?client_id=RealEstate&" +
                $"action=setPickup&" +
                $"dropoff[latitude]={Property.Latitude}&" +
                $"dropoff[longitude]={Property.Longitude}&" +
                $"dropoff[nickname]=Property&" +
                $"dropoff[formatted_address]={WebUtility.UrlEncode(Property.Address)}");
        }

        private async void OpenBrowserAsync(BrowserLaunchMode mode)
        {
            await Browser.OpenAsync(Property.NeighbourhoodUrl, new BrowserLaunchOptions
            {
                LaunchMode = mode,
                TitleMode = BrowserTitleMode.Default,
                PreferredControlColor = Color.White,
                PreferredToolbarColor = Color.FromHex("#2196F3")
            });
        }

        private async void OpenMapsAsync(NavigationMode mode)
        {
            try
            {
                await Map.OpenAsync(Property.Latitude, Property.Longitude, new MapLaunchOptions
                {
                    Name = Property.Address,
                    NavigationMode = mode
                });
            }
            catch(FeatureNotSupportedException ex)
            {
                await DialogService.ShowAlertAsync("No map application installed", "Feature Not Supported");
            }
        }

        private async void SendEmailAsync(Vendor vendor)
        {
            var folder = FileSystem.CacheDirectory;
            var attachmentFilePath = Path.Combine(folder, "draft-contract.pdf");
            using (var packageStream = await FileSystem.OpenAppPackageFileAsync("contract.pdf"))
            {
                using (var fileStream = File.OpenWrite(attachmentFilePath))
                {
                    await packageStream.CopyToAsync(fileStream);
                }
            }

            ExperimentalFeatures.Enable(ExperimentalFeatures.EmailAttachments);

            try
            {
                await Email.ComposeAsync(new EmailMessage
                {
                    To = new List<string> { vendor.Email },
                    Subject = $"Re: {Property.Address}",
                    BodyFormat = EmailBodyFormat.PlainText,
                    Body = $"Hi {vendor.FirstName}, \n\n",
                    Attachments = new List<EmailAttachment>
                    {
                        new EmailAttachment(attachmentFilePath)
                    }
                });
            }
            catch (FeatureNotSupportedException ex)
            {
                await DialogService.ShowAlertAsync("Your device does not support this feature", "Feature Not Supported");
            }
        }

        private async void UsePhoneAsync(Vendor vendor)
        {
            try
            {
                var action = await DialogService.ShowActionSheetAsync(vendor.Phone, "Cancel", null, "Call", "SMS");

                if (action == "Call")
                {
                    PhoneDialer.Open(vendor.Phone);
                }
                else if (action == "SMS")
                {
                    await Sms.ComposeAsync(new SmsMessage
                    {
                        Recipients = new List<string> { vendor.Phone },
                        Body = $"Hi {vendor.FirstName}, regarding {Property.Address} "
                    });
                }
            }
            catch (FeatureNotSupportedException ex)
            {
                await DialogService.ShowAlertAsync("Your device does not support this feature", "Feature Not Supported");
            }
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
        
        private float _selectedVolume = Preferences.Get("SelectedVolume",1f,"TextToSpeech");

        public float SelectedVolume
        {
            get => _selectedVolume;
            set
            {
                SetProperty(ref _selectedVolume, value);
                Preferences.Set("SelectedVolume", value, "TextToSpeech");
            }
        }

        private float _selectedPitch = Preferences.Get("SelectedPitch", 1f, "TextToSpeech");

        public float SelectedPitch
        {
            get => _selectedPitch;
            set
            {
                SetProperty(ref _selectedPitch, value);
                Preferences.Set("SelectedPitch", value, "TextToSpeech");
            }
        }

        private Locale _selectedLocale;

        public Locale SelectedLocale
        {
            get
            {
                if (_selectedLocale == null)
                {
                    var name = Preferences.Get("SelectedLocaleName", null);
                    _selectedLocale = LocalesCollection.FirstOrDefault(x => x.Name == name);
                }
                return _selectedLocale;
            }
            set
            {
                SetProperty(ref _selectedLocale, value);
                Preferences.Set("SelectedLocaleName", value.Name);
            }
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