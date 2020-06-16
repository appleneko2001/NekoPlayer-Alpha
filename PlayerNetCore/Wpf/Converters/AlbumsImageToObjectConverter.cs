using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace NekoPlayer.Wpf.Converters
{
    public class AlbumsImageToObjectConverter : IValueConverter
    {
        private static BitmapImage Default;
        public AlbumsImageToObjectConverter()
        {
            //if (Default is null)
            //    Default = new BitmapImage(new Uri("pack://application:,,,/Wpf/Resources/Album_Default.png"));
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool noDefaultImage = parameter?.ToString().Contains("NoDefault", StringComparison.OrdinalIgnoreCase) ?? false;
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            var path = value as ObservableCollection<BitmapSource>;
            if (path is null)
                throw new ArgumentNullException(null, "Member cannot be converted to collection of bitmapsource.");
            return path.Count >= 1 ? path[0] : (noDefaultImage ? null : new BitmapImage(new Uri(PlayerNetCore.App.DefaultAlbumImageUri)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}