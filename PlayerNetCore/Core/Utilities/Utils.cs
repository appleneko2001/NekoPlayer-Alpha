using MurMur3;
using NekoPlayer.Core.Utilities;
using NekoPlayer.Globalization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
//using LightResize;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls; 
using Image = SixLabors.ImageSharp.Image;
using Rectangle = SixLabors.ImageSharp.Rectangle;
using System.Collections.ObjectModel;
using NekoPlayer.Core;
using ManagedBass;

namespace Appleneko2001
{
    public static class Utils
    {
        /// <summary>
        /// Compare file magic header
        /// </summary>
        /// <param name="origin">File magic header bytes</param>
        /// <param name="target">Target bytes for compare</param>
        /// <param name="offset_origin">Start position, default are zero</param>
        /// <param name="count">Compare length.</param>
        /// <returns>Returns true if match, otherwize will return false.</returns>
        public static bool CompareBytes(byte[] origin, byte[] target, int offset_origin = 0, int count = 1)
        {
            if (target is null || origin is null)
                return true;
            bool result = true;
            for (int i = offset_origin, t = 0; (t < target.Length && i < origin.Length && t < count); i++, t++)
            {
                if (origin[i] != target[t])
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// Extension for string
        /// </summary>
        /// <param name="str">Instance</param>
        /// <returns>Return instance if it are not empty string or null instance otherwize returns null</returns>
        public static string ReturnNullIfEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str;
        }
        public static string ReturnNullIfEmptyAndDeleteNewLines(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str.Replace("\n", " ", StringComparison.InvariantCultureIgnoreCase).Replace("\r", " ", StringComparison.InvariantCultureIgnoreCase);
        }
        public static string CalculateMurmur3Hash(byte[] data)
        {
            var hash = new Murmur3HashCalculation();
            var final = hash.ComputeHash(data);
            return BitConverter.ToString(final);
        }
        /// <summary>
        /// Calculate hash with the Murmur3 fast algorithm. For more information about this algorithm, just visit the page: http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string CalculateMurmur3Hash(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanSeek)
                throw new NotSupportedException("Stream can't to seek, unable to seek old position.");
            var seek = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            var data = new byte[1024 * 8];
            stream.Read(data, 0, data.Length);
            var hash = new Murmur3HashCalculation();
            var final = hash.ComputeHash(data);
            stream.Seek(seek, SeekOrigin.Begin);
            return BitConverter.ToString(final).Replace("-", "", StringComparison.Ordinal);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Проверить аргументы или открытые методы", Justification = "<Ожидание>")]
        public static void CopyFolder(string source, string dest, bool overwrite = true)
        {
            if (!Directory.Exists(source))
                return;
            var files = new List<string>();
            foreach(var node in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
            {
                string s = node.Remove(0, source.Length);
                if (s.StartsWith("\\"))
                    s = s.Remove(0, "\\".Length);
                files.Add(s);
            }
            var exceptions = new List<Exception>();
            foreach(var node in files)
            {
                try
                {
                    string from = Path.Combine(source, node);
                    string destination = Path.Combine(dest, node);
                    if (File.Exists(destination) && overwrite)
                        File.Delete(destination);
                    File.Copy(from, destination);
                }
                catch(Exception e)
                {
                    exceptions.Add(e);
                }
            }
            if (exceptions.Count > 0)
                ExceptMessage.PopupExcept(exceptions,false);
        }
        /// <summary>
        /// Validate the file name are contains the invalid symbols, and replace it automatically.
        /// </summary>
        /// <param name="str">Instance string</param>
        /// <returns>A new file name if we replaced some symbols or return unchanged name.</returns>
        public static string ValidateNameAndFixes(this string str)
        {
            string result = str;
            int pos;
            while ((pos = result?.IndexOfAny(Path.GetInvalidFileNameChars()) ?? -1) != -1)
            {
                string symbol = result.Substring(pos, 1);
                result = result.Replace(symbol, "_", StringComparison.InvariantCulture);
            }
            return result;
        }
        /// <summary>
        /// Show error code when Bass Library returned a false boolean value.
        /// </summary>
        /// <param name="bassCommand">Command from Un4seen.Bass unmanaged wrapper</param>
        /// <param name="isCritical">Is the error in the critical level? Default value is false (warning)</param>
        /// <param name="exception">Show all error but except defined in parameter exception.</param>
        public static void AssertIfFail(bool bassCommand, bool isCritical = false, Errors[] exception = null)
        {
            if (!bassCommand)
            {
                var code = Bass.LastError;
                if (exception != null)
                    if (exception.Contains(code))
                        return;
                ExceptMessage.PopupExcept(code, isCritical);
            }
        }
        public static string GetOpenDialogFilterPattern(string supportedFormatText = "Supported file types")
        {
            string result = "";
            var formats = Analyzer.SupportedFormats;
            List<string> list = new List<string>();
            list.Add(supportedFormatText + "|" + string.Join(";", formats));
            foreach (var f in formats)
            {
                string comment = LanguageManager.RequestNode("filetype" + f.Remove(0, 1));
                list.Add(comment + "|" + f);
            }
            result = string.Join("|", list);
            return result;
        }
        public static bool IsXamlDesignerMode () => System.ComponentModel.DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());

        #region SixLabors.ImageSharp image processing
        /// <summary>
        /// Image resize  
        /// For any reason we take this for image downsize to avoid memory wasting problem
        /// (Yes, we got a 5000x5000 cover image and WPF just loaded it with 80~100MB memory.
        /// NICELY WASTE MEMORY SOLUTION --> just load some images like this and your computer
        /// just overflowing RAM memory.)
        /// So, there I slightly modified method to load from binary data directly and we can
        /// DO MORE, LESS WASTE
        /// and there we used SixLabors library to resize instead because I don't trust
        /// the F***in microsoft windowscodec library
        /// Original solution source: https://stackoverflow.com/questions/7230687/fast-good-quality-image-resizing-algorithm-in-c-sharp-without-using-gdi-wpf
        /// </summary>
        /// <param name="maximumSizeXY">new size image.</param>
        /// <param name="source">the image data source.</param>
        public static byte[] ResizeBitmap(byte[] source, int maximumSizeXY)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            int width = maximumSizeXY, height = maximumSizeXY;
            var outStream = new MemoryStream();
            var image = Image.Load(source);
            if (image.Width > width)
                image.Mutate(context => context.Resize(width, height).Crop(new Rectangle(0, 0, width, height)));
            image.SaveAsPng(outStream);
            image.Dispose();
            image = null;
            byte[] data = (byte[])outStream.GetBuffer().Clone();
            outStream.Close();
            outStream = null;
            GC.Collect();
            return data;
        }
        #endregion
        /*public static Stream ResizeBitmapByLightResize(Stream input, int width = 1000, int height = 1000)
        {
#pragma warning disable CA2000 // Ликвидировать объекты перед потерей области
            Stream output = new MemoryStream();
#pragma warning restore CA2000 // Ликвидировать объекты перед потерей области
            LightResize.ImageBuilder.Build(input, output, true, new LightResize.Instructions { Width = width,Height = height });
            return output;
        }
        public static bool ResizeBitmapByLightResize(Stream input, Stream output, int width = 1000, int height = 1000)
        {
            try
            {
                LightResize.ImageBuilder.Build(input, output, true, new LightResize.Instructions { Width = width, Height = height });
                return true;
            }
            catch
            {
                return false;
            }
        }*/
        public static string GetFullDuration (this ObservableCollection<IPlayable> collection)
        {
            if (collection is null)
                return "#!Null arguments";
            try
            {
                TimeSpan duration = TimeSpan.Zero;
                foreach (var track in collection)
                {
                    if(track.Ready)
                        duration += track.Duration;
                }
                return duration.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                return "#!" + e.Message;
            }
        }
    }
}

namespace MurMur3
{

    public static class IntHelpers
    {
        public static ulong RotateLeft(this ulong original, int bits)
        {
            return (original << bits) | (original >> (64 - bits));
        }

        public static ulong RotateRight(this ulong original, int bits)
        {
            return (original >> bits) | (original << (64 - bits));
        }

        public static ulong GetUInt64(this byte[] bb, int pos)
        {
            // Thanks iron9light provided a improved version for function GetUInt64
            // Source: http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html?showComment=1354868480919#c2111475095152723700
            return BitConverter.ToUInt64(bb, pos);
        }
    }
    public class Murmur3HashCalculation
    {
        // 128 bit output, 64 bit platform version

        public const ulong READ_SIZE = 16;
        private static ulong C1 = 0x87c37b91114253d5L;
        private static ulong C2 = 0x4cf5ad432745937fL;

        private ulong length;
        private const uint seed = 24; // if want to start with a seed, create a constructor
        private ulong h1;
        private ulong h2;

        private void MixBody(ulong k1, ulong k2)
        {
            h1 ^= MixKey1(k1);

            h1 = h1.RotateLeft(27);
            h1 += h2;
            h1 = h1 * 5 + 0x52dce729;

            h2 ^= MixKey2(k2);

            h2 = h2.RotateLeft(31);
            h2 += h1;
            h2 = h2 * 5 + 0x38495ab5;
        }
        private static ulong MixKey1(ulong k1)
        {
            k1 *= C1;
            k1 = k1.RotateLeft(31);
            k1 *= C2;
            return k1;
        }
        private static ulong MixKey2(ulong k2)
        {
            k2 *= C2;
            k2 = k2.RotateLeft(33);
            k2 *= C1;
            return k2;
        }
        private static ulong MixFinal(ulong k)
        {
            // avalanche bits

            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdL;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53L;
            k ^= k >> 33;
            return k;
        }
        public byte[] ComputeHash(byte[] bb)
        {
            if (bb is null)
                ProcessBytes(Array.Empty<byte>());
            else
                ProcessBytes(bb);
            return GetHash();
        }
        private void ProcessBytes(byte[] bb)
        {
            h1 = seed;
            this.length = 0L;

            int pos = 0;
            ulong remaining = (ulong)bb.Length;

            // read 128 bits, 16 bytes, 2 longs in eacy cycle
            while (remaining >= READ_SIZE)
            {
                ulong k1 = bb.GetUInt64(pos);
                pos += 8;

                ulong k2 = bb.GetUInt64(pos);
                pos += 8;

                length += READ_SIZE;
                remaining -= READ_SIZE;

                MixBody(k1, k2);
            }

            // if the input MOD 16 != 0
            if (remaining > 0)
                ProcessBytesRemaining(bb, remaining, pos);
        }
        private void ProcessBytesRemaining(byte[] bb, ulong remaining, int pos)
        {
            ulong k1 = 0;
            ulong k2 = 0;
            length += remaining;

            // little endian (x86) processing
            switch (remaining)
            {
                case 15:
                    k2 ^= (ulong)bb[pos + 14] << 48; // fall through
                    goto case 14;
                case 14:
                    k2 ^= (ulong)bb[pos + 13] << 40; // fall through
                    goto case 13;
                case 13:
                    k2 ^= (ulong)bb[pos + 12] << 32; // fall through
                    goto case 12;
                case 12:
                    k2 ^= (ulong)bb[pos + 11] << 24; // fall through
                    goto case 11;
                case 11:
                    k2 ^= (ulong)bb[pos + 10] << 16; // fall through
                    goto case 10;
                case 10:
                    k2 ^= (ulong)bb[pos + 9] << 8; // fall through
                    goto case 9;
                case 9:
                    k2 ^= bb[pos + 8]; // fall through
                    goto case 8;
                case 8:
                    k1 ^= bb.GetUInt64(pos);
                    break;
                case 7:
                    k1 ^= (ulong)bb[pos + 6] << 48; // fall through
                    goto case 6;
                case 6:
                    k1 ^= (ulong)bb[pos + 5] << 40; // fall through
                    goto case 5;
                case 5:
                    k1 ^= (ulong)bb[pos + 4] << 32; // fall through
                    goto case 4;
                case 4:
                    k1 ^= (ulong)bb[pos + 3] << 24; // fall through
                    goto case 3;
                case 3:
                    k1 ^= (ulong)bb[pos + 2] << 16; // fall through
                    goto case 2;
                case 2:
                    k1 ^= (ulong)bb[pos + 1] << 8; // fall through
                    goto case 1;
                case 1:
                    k1 ^= bb[pos]; // fall through
                    break;
                default:
                    throw new Exception("Something went wrong with remaining bytes calculation.");
            }

            h1 ^= MixKey1(k1);
            h2 ^= MixKey2(k2);
        }
        public byte[] GetHash()
        {
            h1 ^= length;
            h2 ^= length;

            h1 += h2;
            h2 += h1;

            h1 = MixFinal(h1);
            h2 = MixFinal(h2);

            h1 += h2;
            h2 += h1;

            var hash = new byte[READ_SIZE];

            Array.Copy(BitConverter.GetBytes(h1), 0, hash, 0, 8);
            Array.Copy(BitConverter.GetBytes(h2), 0, hash, 8, 8);

            return hash;
        }
    }
}

namespace RachelLim
{
    /// <summary>
    /// A Grid utility that can dynamically changing column and row definitions from Rachel Lim
    /// Link: https://rachel53461.wordpress.com/2011/09/17/wpf-grids-rowcolumn-count-properties/
    /// </summary>
    public static class GridHelpers
    {
        #region RowCount Property

        /// <summary>
        /// Adds the specified number of Rows to RowDefinitions. 
        /// Default Height is Auto
        /// </summary>
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.RegisterAttached(
                "RowCount", typeof(int), typeof(GridHelpers),
                new PropertyMetadata(-1, RowCountChanged));

        // Get
        public static int GetRowCount(DependencyObject obj)
        {
            return (int)obj?.GetValue(RowCountProperty);
        }

        // Set
        public static void SetRowCount(DependencyObject obj, int value)
        {
            obj?.SetValue(RowCountProperty, value);
        }

        // Change Event - Adds the Rows
        public static void RowCountChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || (int)e.NewValue < 0)
                return;

            Grid grid = (Grid)obj;
            grid.RowDefinitions.Clear();

            for (int i = 0; i < (int)e.NewValue; i++)
                grid.RowDefinitions.Add(
                    new RowDefinition() { Height = GridLength.Auto });

            SetStarRows(grid);
        }

        #endregion

        #region ColumnCount Property

        /// <summary>
        /// Adds the specified number of Columns to ColumnDefinitions. 
        /// Default Width is Auto
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.RegisterAttached(
                "ColumnCount", typeof(int), typeof(GridHelpers),
                new PropertyMetadata(-1, ColumnCountChanged));

        // Get
        public static int GetColumnCount(DependencyObject obj)
        {
            return (int)obj?.GetValue(ColumnCountProperty);
        }

        // Set
        public static void SetColumnCount(DependencyObject obj, int value)
        {
            obj?.SetValue(ColumnCountProperty, value);
        }

        // Change Event - Add the Columns
        public static void ColumnCountChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || (int)e.NewValue < 0)
                return;

            Grid grid = (Grid)obj;
            grid.ColumnDefinitions.Clear();

            for (int i = 0; i < (int)e.NewValue; i++)
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition() { Width = GridLength.Auto });

            SetStarColumns(grid);
        }

        #endregion

        #region StarRows Property

        /// <summary>
        /// Makes the specified Row's Height equal to Star. 
        /// Can set on multiple Rows
        /// </summary>
        public static readonly DependencyProperty StarRowsProperty =
            DependencyProperty.RegisterAttached(
                "StarRows", typeof(string), typeof(GridHelpers),
                new PropertyMetadata(string.Empty, StarRowsChanged));

        // Get
        public static string GetStarRows(DependencyObject obj)
        {
            return (string)obj?.GetValue(StarRowsProperty);
        }

        // Set
        public static void SetStarRows(DependencyObject obj, string value)
        {
            obj?.SetValue(StarRowsProperty, value);
        }

        // Change Event - Makes specified Row's Height equal to Star
        public static void StarRowsChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
                return;

            SetStarRows((Grid)obj);
        }

        #endregion

        #region StarColumns Property

        /// <summary>
        /// Makes the specified Column's Width equal to Star. 
        /// Can set on multiple Columns
        /// </summary>
        public static readonly DependencyProperty StarColumnsProperty =
            DependencyProperty.RegisterAttached(
                "StarColumns", typeof(string), typeof(GridHelpers),
                new PropertyMetadata(string.Empty, StarColumnsChanged));

        // Get
        public static string GetStarColumns(DependencyObject obj)
        {
            return (string)obj?.GetValue(StarColumnsProperty);
        }

        // Set
        public static void SetStarColumns(DependencyObject obj, string value)
        {
            obj?.SetValue(StarColumnsProperty, value);
        }

        // Change Event - Makes specified Column's Width equal to Star
        public static void StarColumnsChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || string.IsNullOrEmpty(e.NewValue.ToString()))
                return;

            SetStarColumns((Grid)obj);
        }

        #endregion

        private static void SetStarColumns(Grid grid, string cols = "")
        {
            string[] starColumns = GetStarColumns(grid).Split(',');
            //string[] colsData = cols.Split(',');

            for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                if (starColumns.Contains(i.ToString(CultureInfo.InvariantCulture)))
                    grid.ColumnDefinitions[i].Width = new GridLength(1, GridUnitType.Star);
                //double width = 0;
                /*if (i < colsData.Length)
                {
                    if (double.TryParse(colsData[i], out width))
                        grid.ColumnDefinitions[i].Width = new GridLength(width, GridUnitType.Star);
                }*/
            }
        }

        private static void SetStarRows(Grid grid)
        {
            string[] starRows =
                GetStarRows(grid).Split(',');

            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                if (starRows.Contains(i.ToString(CultureInfo.InvariantCulture)))
                    grid.RowDefinitions[i].Height =
                        new GridLength(1, GridUnitType.Star);
            }
        }
    }
}