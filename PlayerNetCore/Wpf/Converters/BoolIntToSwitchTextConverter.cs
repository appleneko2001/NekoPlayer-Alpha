using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NekoPlayer.Wpf.Converters
{
    public class ITSVariant
    {
        public ITSVariant (string[] strings)
        {
            Strings = strings;
        }
        public string[] Strings;
    }
    public class BoolToSwitchTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool)
                if(parameter is ITSVariant)
                {
                    var variant = parameter as ITSVariant;
                    if(variant.Strings.Length > 1)
                        return variant.Strings[(bool)value ? 1 : 0];
                    else
                        return variant.Strings[0];
                }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class IntToSwitchTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
                if (parameter is ITSVariant)
                {
                    var variant = parameter as ITSVariant;
                    if (variant.Strings.Length > 1)
                        return variant.Strings[(int)value];
                    else
                        return variant.Strings[0];
                }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
