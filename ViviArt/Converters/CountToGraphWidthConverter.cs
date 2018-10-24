using System;
using System.Globalization;
using Xamarin.Forms;

namespace ViviArt
{

    public class CountToGraphWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value * 70;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value / 70;
        }
    }
}
