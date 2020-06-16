using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NekoPlayer.Wpf.Converters
{
    public class StringFormatMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null)
                return "";
            if(parameter is string)
            {
                return string.Format(CultureInfo.InvariantCulture, parameter as string, values);
            }
            else
            {
                string fullText = "";
                foreach(var item in values)
                {
                    fullText += item;
                }
                return fullText;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
