using RealEstateApp.Services.Login;
using RealEstateApp.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RealEstateApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ILoginService _loginService;

        public LoginViewModel(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public ICommand LoginCommand => new Command(LoginAsync);

        private async void LoginAsync()
        {
            IsBusy = true;
            StatusMessage = null;

            var result = await _loginService.Login(Username, Password);

            if (result.Succeeded)
            {
                await SecureStorage.SetAsync("AccessToken", result.AccessToken);
                await SecureStorage.SetAsync("RefreshToken", result.RefreshToken);

                await NavigationService.NavigateToAsync<PropertyListViewModel>();
            }
            else
            {
                StatusMessage = "Invalid username/password";
                Password = null;
            }

            IsBusy = false;
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData?.ToString() == "logout")
            {
                SecureStorage.RemoveAll();
            }

            var token = await SecureStorage.GetAsync("AccessToken");

            if (await _loginService.IsValidAccessToken(token))
            {
                await NavigationService.NavigateToAsync<PropertyListViewModel>();
            }
        }

        private string _username;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _statusMessage;

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }
    }
}