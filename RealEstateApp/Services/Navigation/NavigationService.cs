using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using RealEstateApp.ViewModels;
using RealEstateApp.ViewModels.Base;
using RealEstateApp.Views;
using Xamarin.Forms;

namespace RealEstateApp.Services.Navigation
{
    public class NavigationService : INavigationService
    {
        public Task InitializeAsync()
        {
            Application.Current.MainPage = new MainView();
            return NavigateToAsync<PropertyListViewModel>();
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public async Task<TViewModel> NavigateToModalAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            return (TViewModel)(await InternalNavigateToAsync(typeof(TViewModel), null, true));
        }

        public async Task<TViewModel> NavigateToModalAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return (TViewModel)(await InternalNavigateToAsync(typeof(TViewModel), parameter, true));
        }

        public Task NavigateToAsync(Type viewModelType, object parameter)
        {
            return InternalNavigateToAsync(viewModelType, parameter);
        }

        public Task PopModalAsync()
        {
            var mainPage = Application.Current.MainPage as MainView;
            mainPage.Navigation.PopModalAsync();
            return Task.CompletedTask;
        }

        private async Task<ViewModelBase> InternalNavigateToAsync(Type viewModelType, object parameter, bool isModal = false)
        {
            var page = CreatePage(viewModelType, parameter);

            var viewModel = page.BindingContext as ViewModelBase;

            if (viewModel != null) SubscribeToPageLifecycle(page, viewModel);

            if (page is MainView)
            {
                Application.Current.MainPage = page;
            }
            else if (Application.Current.MainPage is MainView)
            {
                var mainPage = Application.Current.MainPage as MainView;
                var navigationPage = mainPage.Detail as NavigationPage;

                if (MenuViewModel.IsMenuItem(viewModelType) || navigationPage == null)
                {
                    navigationPage = new NavigationPage(page);
                    mainPage.Detail = navigationPage;
                }
                else
                {
                    if (isModal)
                        await navigationPage.Navigation.PushModalAsync(new NavigationPage(page));
                    else
                        await navigationPage.PushAsync(page);
                }

                mainPage.IsPresented = false;
            }

            await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);

            return viewModel;
        }

        private Type GetPageTypeForViewModel(Type viewModelType)
        {
            var viewName = viewModelType.FullName.Replace("Model", string.Empty);
            var viewModelAssemblyName = viewModelType.GetTypeInfo().Assembly.FullName;
            var viewAssemblyName =
                string.Format(CultureInfo.InvariantCulture, "{0}, {1}", viewName, viewModelAssemblyName);
            var viewType = Type.GetType(viewAssemblyName);
            return viewType;
        }

        private Page CreatePage(Type viewModelType, object parameter)
        {
            var pageType = GetPageTypeForViewModel(viewModelType);
            if (pageType == null) throw new Exception($"Cannot locate page type for {viewModelType}");

            var page = Activator.CreateInstance(pageType) as Page;
            return page;
        }

        private void SubscribeToPageLifecycle(Page page, ViewModelBase viewModel)
        {
            page.Appearing += OnPageAppearing;
            page.Disappearing += OnPageDisappearing;
        }

        private void OnPageDisappearing(object sender, EventArgs e)
        {
            var page = (Page) sender;

            page.Appearing -= OnPageAppearing;
            page.Disappearing -= OnPageDisappearing;

            var viewModel = page.BindingContext as ViewModelBase;
            viewModel?.OnDisappearing();
        }

        private void OnPageAppearing(object sender, EventArgs e)
        {
            var page = (Page) sender;
            var viewModel = page.BindingContext as ViewModelBase;
            viewModel?.OnAppearing();
        }
    }
}