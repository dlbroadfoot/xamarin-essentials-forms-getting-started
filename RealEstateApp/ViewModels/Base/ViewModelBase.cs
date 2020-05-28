using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RealEstateApp.Services.Dialog;
using RealEstateApp.Services.Logging;
using RealEstateApp.Services.Navigation;
using RealEstateApp.Services.Repository;
using Xamarin.Forms;

namespace RealEstateApp.ViewModels.Base
{
    public abstract class ViewModelBase : BindableObject
    {
        protected readonly INavigationService NavigationService;
        protected readonly IDialogService DialogService;
        protected readonly IRepository Repository;
        protected readonly ILogger Logger;

        private bool _isBusy;

        private string _title;

        protected ViewModelBase()
        {
            NavigationService = ViewModelLocator.Resolve<INavigationService>();
            DialogService = ViewModelLocator.Resolve<IDialogService>();
            Repository = ViewModelLocator.Resolve<IRepository>();
            Logger = ViewModelLocator.Resolve<ILogger>();
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null, bool forcePropertyChangedNotification = false)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                if (forcePropertyChangedNotification) OnPropertyChanged(propertyName);
                return false;
            }

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }

        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }
    }
}