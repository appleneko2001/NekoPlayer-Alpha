using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NekoPlayer.Wpf.Converters
{
    public class ITPVariants
    {
        public ITPVariants(PackIconKind[] kinds)
        {
            Kinds = kinds;
        }
        public PackIconKind[] Kinds = Array.Empty<PackIconKind>();
    }
    public class IntToPackIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is ITPVariants)
                if (value is int)
                    return ((ITPVariants)parameter).Kinds[(int)value];
            return PackIconKind.RemoveCircle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
