using System;
using System.Globalization;
using System.Windows.Data;

namespace SluggaManager
{
    public class LockTimestampConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Set the time zone information for Pacific Time and Eastern Time
            TimeZoneInfo pacificZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            string dt = (string)value;
            if (string.IsNullOrWhiteSpace(dt)) { return dt; }
            var pactime = DateTime.Parse(dt, CultureInfo.InvariantCulture);

            // Convert the Pacific Time to Eastern Time
            DateTime easternTime = TimeZoneInfo.ConvertTime(pactime, pacificZone, easternZone);

            return $"{easternTime:M/d/yy hh:mm tt}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
