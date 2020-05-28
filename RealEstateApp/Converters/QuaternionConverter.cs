using System;
using System.Globalization;
using System.Numerics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Converters
{
    public class QuaternionConverter : IValueConverter, IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (Quaternion)value;

            return $"(X: {data.X:N2}, Y: {data.Y:N2}, Z: {data.Z:N2}, W: {data.W:N2})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
