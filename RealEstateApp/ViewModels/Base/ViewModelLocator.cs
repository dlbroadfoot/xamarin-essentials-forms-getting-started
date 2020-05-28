using System;
using System.Globalization;
using System.Reflection;
using RealEstateApp.Services.Dialog;
using RealEstateApp.Services.Logging;
using RealEstateApp.Services.Login;
using RealEstateApp.Services.Navigation;
using RealEstateApp.Services.Repository;
using RealEstateApp.Services.Settings;
using RealEstateApp.Services.Updates;
using TinyIoC;
using Xamarin.Forms;

namespace RealEstateApp.ViewModels.Base
{
    public static class ViewModelLocator
    {
        private static readonly TinyIoCContainer _container;

        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool),
                propertyChanged: OnAutoWireViewModelChanged);

        static ViewModelLocator()
        {
            _container = new TinyIoCContainer();

            // View models - by default, TinyIoC will register concrete classes as multi-instance.
            _container.Register<AboutViewModel>();
            _container.Register<AddEditPropertyViewModel>();
            _container.Register<MenuViewModel>();
            _container.Register<MainViewModel>();
            _container.Register<PropertyDetailViewModel>();
            _container.Register<PropertyListViewModel>();

            // Services - by default, TinyIoC will register interface registrations as singletons.
            _container.Register<INavigationService, NavigationService>();
            _container.Register<IRepository, MockRepository>();
            _container.Register<ISettingsService, SettingsService>();
            _container.Register<IDialogService, DialogService>();
            _container.Register<ILogger, DebugLogger>();
            _container.Register<ILoginService, MockLoginService>();
            _container.Register<IUpdateService, UpdateService>();
        }

        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool) bindable.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(AutoWireViewModelProperty, value);
        }

        public static T Resolve<T>() where T : class
        {
            return _container.Resolve<T>();
        }

        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as Element;
            if (view == null) return;

            var viewType = view.GetType();
            var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var viewModelName =
                string.Format(CultureInfo.InvariantCulture, "{0}Model, {1}", viewName, viewAssemblyName);

            var viewModelType = Type.GetType(viewModelName);
            if (viewModelType == null) return;
            var viewModel = _container.Resolve(viewModelType);
            view.BindingContext = viewModel;
        }
    }
}