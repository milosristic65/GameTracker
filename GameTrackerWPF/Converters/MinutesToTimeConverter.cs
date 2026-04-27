using System.Globalization;
using System.Windows.Data;

namespace GameTrackerWPF.Converters
{
    public class MinutesToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint minutes = (uint)value;
            uint hours = minutes / 60;
            uint mins = minutes % 60;

            if (hours == 0) return $"{mins}m";
            if (mins == 0) return $"{hours}h";
            return $"{hours}h {mins}m";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string time = value.ToString() ?? "";
            uint totalMinutes = 0;

            // Has both hours and minutes
            if (time.Contains('h'))
            {
                var parts = time.Split('h');
                if (uint.TryParse(parts[0].Trim(), out uint hours))
                {
                    totalMinutes += hours * 60;
                }

                if (parts.Length > 1 && parts[1].Contains('m'))
                {
                    var minPart = parts[1].Replace("m", "").Trim();
                    if (uint.TryParse(minPart, out uint mins))
                    {
                        totalMinutes += mins;
                    }
                }
            }
            // Has just minutes
            else if (time.Contains('m'))
            {
                var minPart = time.Replace("m", "").Trim();
                if (uint.TryParse(minPart, out uint mins))
                {
                    totalMinutes = mins;
                }
            }

            return totalMinutes;
        }
    }
}
