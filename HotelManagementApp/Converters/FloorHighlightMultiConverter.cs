using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HotelManagementApp.Converters
{
    public class FloorHighlightMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Brushes.Gray;

            int floor = System.Convert.ToInt32(values[0]);
            int selected = System.Convert.ToInt32(values[1]);

            return floor == selected
                ? new SolidColorBrush(Color.FromRgb(103, 58, 183))
                : Brushes.LightGray;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
