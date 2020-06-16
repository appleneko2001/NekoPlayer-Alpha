using NekoPlayer.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NekoPlayer.Wpf.Converters
{
    public class TrackAlbumIllustConverter : IValueConverter
    {
        //private static BitmapImage Default;
        public TrackAlbumIllustConverter()
        {
            //if(Default is null)
            //    Default = new BitmapImage(new Uri("pack://application:,,,/Wpf/Resources/Album_Default.png"));
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
            //if (value is TagInfo)
               
            /*    return value == null ?
                     new BitmapImage(new Uri(PlayerNetCore.App.DefaultAlbumImageUri))
                     : value;
            else if (value is ObservableCollection<BitmapSource> && (value as ObservableCollection<BitmapSource>).Count > 0)
            {
                return value == null ?
                     new BitmapImage(new Uri(PlayerNetCore.App.DefaultAlbumImageUri))
                     : (value as ObservableCollection<BitmapSource>)[0];
            }
            else
                return new BitmapImage(new Uri(PlayerNetCore.App.DefaultAlbumImageUri));*/
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
