using System.Threading.Tasks;
using RealEstateApp.ViewModels.Base;

namespace RealEstateApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(MenuViewModel menuViewModel)
        {
            _menuViewModel = menuViewModel;
        }

        private MenuViewModel _menuViewModel;

        public MenuViewModel MenuViewModel
        {
            get => _menuViewModel;
            set => SetProperty(ref _menuViewModel, value);
        }

        public override Task InitializeAsync(object navigationData)
        {
            return Task.WhenAll
            (
                _menuViewModel.InitializeAsync(navigationData),
                NavigationService.NavigateToAsync<PropertyListViewModel>()
            );
        }
    }
}