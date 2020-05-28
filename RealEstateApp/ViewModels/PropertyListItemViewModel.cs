using RealEstateApp.Models;
using RealEstateApp.ViewModels.Base;
using Xamarin.Essentials;

namespace RealEstateApp.ViewModels
{
    public class PropertyListItemViewModel : ViewModelBase
    {
        private double? _distanceInKilometres;

        public double? DistanceInKilometres
        {
            get => _distanceInKilometres;
            set
            {
                SetProperty(ref _distanceInKilometres, value);
                _distanceInMiles = UnitConverters.KilometersToMiles(_distanceInKilometres.GetValueOrDefault());
            }
        }
        private double? _distanceInMiles;

        public double? DistanceInMiles
        {
            get => _distanceInMiles;
            set
            {
                SetProperty(ref _distanceInMiles, value);
                _distanceInKilometres = UnitConverters.MilesToKilometers(_distanceInMiles.GetValueOrDefault());
            }
        }

        public PropertyListItemViewModel(Property property)
        {
            Property = property;
        }

        private Property _property;

        public Property Property
        {
            get => _property;
            set => SetProperty(ref _property, value);
        }
    }
}