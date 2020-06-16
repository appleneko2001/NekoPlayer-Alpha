using NekoPlayer.Wpf.ModelViews;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NekoPlayer.Wpf.Converters
{
    public class GetPlayerContextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GlobalViewModel.GetInstance();// ?? new GlobalViewModel();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
