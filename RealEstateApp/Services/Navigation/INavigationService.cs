using System;
using System.Threading.Tasks;
using RealEstateApp.ViewModels.Base;

namespace RealEstateApp.Services.Navigation
{
    public interface INavigationService
    {
        Task InitializeAsync();

        Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase;

        Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        Task<TViewModel> NavigateToModalAsync<TViewModel>() where TViewModel : ViewModelBase;

        Task<TViewModel> NavigateToModalAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        Task NavigateToAsync(Type viewModelType, object parameter);

        Task PopModalAsync();
    }
}