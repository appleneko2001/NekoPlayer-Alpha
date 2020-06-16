using LinqToDB.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Documents;

namespace NekoPlayer.Wpf.Converters
{
    public class ToByteSizeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int | value is long)
            {
                long data = (long)value;
                string text = "N/A";
                if(data >= 0 && data < 1000)
                {
                    text = $"{data} bytes";
                }
                else if(data >= 1000 && data < 1000000)
                {
                    double d = Math.Round(data / 1024.0, 2, MidpointRounding.ToEven);
                    text = $"{d} Kbytes";
                }
                else if (data >= 1000000)
                {
                    double d = Math.Round(data / 1024.0 / 1024.0, 2, MidpointRounding.ToEven);
                    text = $"{d} Mbytes";
                }
                return text;
            }
            else
                return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
