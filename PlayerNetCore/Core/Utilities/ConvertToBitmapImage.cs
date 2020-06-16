//https://stackoverflow.com/questions/30727343/fast-converting-bitmap-to-bitmapsource-wpf
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NekoPlayer.Core
{
    public static class Extension_BitmapToBitmapSource
    {
        public static BitmapSource Convert(this System.Drawing.Bitmap bitmap)
        {
            if (bitmap is null)
                throw new System.ArgumentNullException(nameof(bitmap));
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
            PixelFormat pixelFormats = ConvertToWPFFormat(bitmapData.PixelFormat);
            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                pixelFormats, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
        }
        private static PixelFormat ConvertToWPFFormat(System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return PixelFormats.Bgr24;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return PixelFormats.Bgra32;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    return PixelFormats.Bgr32;
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    return PixelFormats.Pbgra32;
            }
            return new PixelFormat();
        }
    }
}
