using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace NekoPlayer.Wpf.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
            return value is null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// An complex boolean or integer compare converter for visibility
    /// Usage: [bool|int|==|<=|>=|<|>|!=|true|setting key|compare value]
    /// Example: int >= 10 will Compare if value are greater or equal 10
    /// Default comparing condition: Boolean, Equals, True
    /// </summary>
    public class VisibilityBySettingsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = null;
            int compareWith = 1; // 1 is means true if your mode is bool
            int mode = 0; //0 - Bool 1 - Int
            int compareType = 0; // 0 - Equals, 1 - Less or Equals, 2 - Great or Equals
            // 3 - Less than, 4 - Great than, 5 - Not Equals
            if(parameter is string)
            {
                string[] param = (parameter as string).Replace(" ", "",
                    StringComparison.OrdinalIgnoreCase).Split(',');
                foreach(var p in param)
                {
                    if (p.Contains("bool", StringComparison.InvariantCultureIgnoreCase))
                        mode = 0;
                    else if (p.Contains("int", StringComparison.InvariantCultureIgnoreCase))
                        mode = 1;
                    else if (p.Contains("==", StringComparison.InvariantCultureIgnoreCase))
                        compareType = 0;
                    else if (p.Contains("<=", StringComparison.InvariantCultureIgnoreCase))
                        compareType = 1;
                    else if (p.Contains(">=", StringComparison.InvariantCultureIgnoreCase))
                        compareType = 2;
                    else if (p.Contains("<", StringComparison.InvariantCultureIgnoreCase))
                        compareType = 3;
                    else if (p.Contains(">", StringComparison.InvariantCultureIgnoreCase))
                        compareType = 4;
                    else if (p.Contains("!=", StringComparison.InvariantCultureIgnoreCase))
                        compareType = 5;
                    else if (p.Contains("true", StringComparison.InvariantCultureIgnoreCase))
                        compareWith = 1;
                    else if (!char.IsDigit(p[0]))
                        path = p;
                    else if (int.TryParse(p, out compareWith))
                        continue;
                }
            }
            if (path is string)
            {
                bool result = false;
                if (mode == 0)
                {
                    var r_bool = SettingsManager.GetValue<bool>(path as string) ? 1 : 0;
                    result = (compareType == 5 ? (compareWith != r_bool) : (compareWith == r_bool));
                }
                else if (mode == 1)
                {
                    var r_int = SettingsManager.GetValue<int>(path as string);
                    result =
                        (compareType == 0 ? (compareWith == r_int) : 
                        (compareType == 1 ? (compareWith <= r_int) :
                        (compareType == 2 ? (compareWith >= r_int) :
                        (compareType == 3 ? (compareWith < r_int)  :
                        (compareType == 4 ? (compareWith > r_int)  :
                        (compareWith == 5 ? (compareWith != r_int) : false))))));
                }
                return result ? Visibility.Visible : Visibility.Collapsed;
            }
            return path is null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
