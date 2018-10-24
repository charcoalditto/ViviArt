using System;
using System.Globalization;
using Xamarin.Forms;

namespace ViviArt
{

    public class StringToDateTime : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value).ToDateTime1();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString1();
        }
    }
}
