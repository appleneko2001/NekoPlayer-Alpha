using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NekoPlayer.Wpf.Converters
{
    public class GetSettingsIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string) 
            {
                int v = 0;
                if (int.TryParse(SettingsManager.GetValue<string>(parameter.ToString(), ""), out v))
                {
                    return v;
                } 
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
