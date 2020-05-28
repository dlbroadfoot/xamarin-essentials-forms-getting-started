using RealEstateApp.ViewModels.Base;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace RealEstateApp.ViewModels
{
    public class CompassViewModel : ViewModelBase
    {
        public CompassViewModel()
        {
        }

        public override void OnAppearing()
        {

        }

        public override void OnDisappearing()
        {

        }

        private string _currentAspect;

        public string CurrentAspect
        {
            get => _currentAspect;
            set => SetProperty(ref _currentAspect, value);
        }

        private double _currentHeading;

        public double CurrentHeading
        {
            get => _currentHeading;
            set => SetProperty(ref _currentHeading, value);
        }

        private double _rotation;

        public double Rotation
        {
            get => _rotation;
            set => SetProperty(ref _rotation, value);
        }

        public Task<string> GetAspectAsync()
        {
            _getAspectTask = new TaskCompletionSource<string>();

            return _getAspectTask.Task;
        }

        private TaskCompletionSource<string> _getAspectTask;

        private void Close(bool cancel)
        {
            if (cancel)
            {
                _getAspectTask?.SetResult(null);
            }
            else
            {
                _getAspectTask?.SetResult(CurrentAspect);
            }

            NavigationService.PopModalAsync();
        }

        public ICommand CloseCommand => new Command<bool>(Close);
    }
}
