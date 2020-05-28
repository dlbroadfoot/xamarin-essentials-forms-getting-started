using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Converters
{
    public class HeightChangeConverter : IValueConverter, IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = (double)value;

            if (targetType == typeof(Color))
            {
                return number >= 0 ? Color.Green : Color.Red;
            }
            else if (targetType == typeof(bool))
            {
                return number != 0;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}