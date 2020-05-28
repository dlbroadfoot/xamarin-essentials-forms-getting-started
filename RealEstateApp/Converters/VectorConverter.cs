using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RealEstateApp.Converters
{
    public class VectorConverter : IValueConverter, IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (Vector3)value;

            var xLabel = "X";
            var yLabel = "Y";
            var zLabel = "Z";

            if (parameter != null)
            {
                var labels = parameter.ToString().Split(',');

                if (labels.Length == 3)
                {
                    xLabel = labels[0];
                    yLabel = labels[1];
                    zLabel = labels[2];
                }
            }

            return $"({xLabel}: {data.X:N2}, {yLabel}: {data.Y:N2}, {zLabel}: {data.Z:N2})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
