using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace HotelManagementApp.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                "empty" => Brushes.White,
                "in_use" => Brushes.Green,
                "cleaning" => Brushes.Gold,
                "repairing" => Brushes.Gray,
                "overdue" => Brushes.Red,
                _ => Brushes.White
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
