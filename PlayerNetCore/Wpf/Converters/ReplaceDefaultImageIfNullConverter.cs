using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace NekoPlayer.Wpf.Converters
{
    public class ReplaceDefaultImageIfNullConverter : IValueConverter
    {
        private static BitmapImage Default;
        public ReplaceDefaultImageIfNullConverter()
        {
            //if(Default is null)
            //    Default = new BitmapImage(new Uri("pack://application:,,,/Wpf/Resources/Album_Default.png"));
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool noDefaultImage = parameter?.ToString().Contains("NoDefault", StringComparison.OrdinalIgnoreCase) ?? false;
            if (value is BitmapSource)
                return value == null ? (noDefaultImage ? null : new BitmapImage(new Uri(PlayerNetCore.App.DefaultAlbumImageUri)))
                     : value;
            else if(value is ObservableCollection<BitmapSource> && (value as ObservableCollection<BitmapSource>).Count > 0)
            {
                return value == null ?
                     (noDefaultImage ? null : new BitmapImage(new Uri(PlayerNetCore.App.DefaultAlbumImageUri)))
                     : (value as ObservableCollection<BitmapSource>)[0];
            }
            else
                return (noDefaultImage ? null : new BitmapImage(new Uri(PlayerNetCore.App.DefaultAlbumImageUri)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
