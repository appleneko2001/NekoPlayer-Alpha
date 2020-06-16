using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NekoPlayer.Wpf.Converters
{
    public class BTPVariants
    {
        public BTPVariants(PackIconKind ifTrue, PackIconKind ifFalse)
        {
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }
        public PackIconKind IfTrue = PackIconKind.CheckBold;
        public PackIconKind IfFalse = PackIconKind.CloseOutline;
    }
    public class BoolToPackIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PackIconKind result = PackIconKind.Null;
            if(parameter is BTPVariants)
            {
                bool? b = value as bool?;
                var p = (parameter as BTPVariants);
                if(b != null)
                    result = (b.Value ? p.IfTrue : p.IfFalse);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
