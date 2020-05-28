using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Converters
{
    public class ForegroundColorConverter : IValueConverter, IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color) value;

            var brightness = GetBrightness(color);

            var foregroundColor = brightness < 125 ? Color.White : Color.Black;

            return foregroundColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private int GetBrightness(Color c)
        {
            return (int) ((c.R * 299 + c.G * 587 + c.B * 114) / 3.92);
        }
    }
}