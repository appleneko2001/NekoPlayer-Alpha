using Appleneko2001; //Extensions
using NekoPlayer.Containers;
using NekoPlayer.Networking;
using NekoPlayer.Wpf.ModelViews;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TagLib;

namespace NekoPlayer.Core
{
    public enum TagSourceEnum
    {
        Network,
        FileTagContainer,
        Filename
    }
    public class TagInfo : ViewModelBase
    {
        // Base tagged music infos
        public string TrackId { get; private set; }
        public string Artist { get; private set; }
        public string ArtistAlbum { get; private set; }
        public string Album { get; private set; }
        public string Year { get; private set; }
        public string Genre { get; private set; }
        public string Title { get; private set; }
        public string TagProvider { get; private set; }

        public string NetworkLink_AlbumImage { get; private set; }
        Playable thisPlayable;
        private long? songId, albumImageId;
        //public Exception Error_AlbumImage { get; private set; }
        public string AlbumIllustErrorInfo { get; private set; }
        //public TimeSpan Duration;
        public BitmapSource AlbumImage { 
            get {
                BitmapSource returns = GetCoverCache();
                if (returns is null)
                {
                    try
                    {
                        using (TagLib.File taglib = TagLib.File.Create(thisPlayable.GetMediaPath(), Analyzer.GetMimetypeString(thisPlayable.MediaFormat), TagLib.ReadStyle.None))
                        {
                            var pics = taglib.Tag.Pictures;
                            if (pics.Length >= 1)
                            {
                                try
                                {
                                    int index = pics.Length - 1; // Pick the last one
                                    returns = GetAlbumIllust(pics[index].Data.Data);
                                }
                                catch (Exception e)
                                {
                                    AlbumIllustErrorInfo = e.Message;
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {

                    }
                }
                return returns;
            } 
        }
        public TagSourceEnum TagSource { get; private set; }
        /// <summary>
        /// Parse data from Taglib Audio Tag
        /// </summary>
        /// <param name="tag">Instance Taglib tag object</param>
        public TagInfo(TagLib.Tag tag, Playable playable)
        {
            thisPlayable = playable;
            TagSource = TagSourceEnum.FileTagContainer;
            if (tag is null)
                throw new ArgumentNullException(nameof(tag));
            Artist = tag.FirstPerformer.ReturnNullIfEmptyAndDeleteNewLines();
            ArtistAlbum = tag.FirstAlbumArtist.ReturnNullIfEmptyAndDeleteNewLines();
            Album = tag.Album.ReturnNullIfEmptyAndDeleteNewLines();
            Year = (tag.Year == 0) ? null : tag.Year.ToString((IFormatProvider)null); // Do not use "0" or similar thing if we don't know, just keep it "null"
            Genre = tag.FirstGenre.ReturnNullIfEmptyAndDeleteNewLines();
            Title = tag.Title.ReturnNullIfEmptyAndDeleteNewLines();
            if (tag.Track != 0)
            TrackId = tag.Track.ToString(CultureInfo.InvariantCulture);
            NetworkLink_AlbumImage = null;
            //GetAlbumIllust(tag.Pictures);
            GetCache();
            OnPropertyChanged();
        }
        /// <summary>
        /// Just create a empty tag container, you can fill infos later with other source.
        /// </summary>
        public TagInfo()
        {

        }
        public void GetCache()
        {
            try
            {
                if (CacheManager.ContainLink(thisPlayable.GetObjectHash()))
                {
                    songId = CacheManager.GetLinkCache(thisPlayable.GetObjectHash());
                    var data = CacheManager.GetNetworkTagCache(songId ?? 0);
                    Title = data?.Title;
                    Artist = data?.Artist;
                    Album = data?.Album;
                    ArtistAlbum = data?.ArtistAlbum;
                    albumImageId = data?.AlbumImageId;
                    TagProvider = data?.TagProvider;
                    NetworkLink_AlbumImage = data?.AlbumImageLink;
                    Year = data?.Year;
                    //ApplyAlbumIllust(CacheManager.GetImageCache(songId));
                }
            }
            catch
            {

            }
        }

        public void ApplyTitle(string name)
        {
            Title = name;
            NotifyUpdate("Title");
        }
        public void ApplyTagInfo(GetMusicInfoResult result)
        {
            Title = result?.Title;
            Artist = result?.Artist;
            Album = result?.Album;
            ArtistAlbum = result?.AlbumArtist;
            TagProvider = result?.Provider;
            NetworkLink_AlbumImage = result?.AlbumIllustrationLink;
            Year = result?.Year;
            songId = result?.SongId;
            albumImageId = result?.AlbumIllustId;
            CacheManager.PushNetworkTagCache(result);
            CacheManager.LinkCache(thisPlayable.GetObjectHash(), songId is null ? 0 : songId.Value);
            NotifyUpdate();
        }
        public async void DownloadAlbumIllust()
        {
            using (WebClient wc = new WebClient())
            {
                var data = await wc.DownloadDataTaskAsync(NetworkLink_AlbumImage).ConfigureAwait(false);
                if (SettingsManager.GetValue<bool>("DownsizeWhenGetCoverOnline", false))
                {
                    // Image resize technology by LightResize
                    /*using (MemoryStream input = new MemoryStream(data))
                    {
                        using (MemoryStream resized = new MemoryStream())
                        {
                            Utils.ResizeBitmapByLightResize(input, resized);
                            data = resized.GetBuffer();
                        }
                    }*/
                    data = Utils.ResizeBitmap(data, 1000);//  SixLabors resize
                }
                CacheManager.PushImageCache(songId == null ? 0 : songId.Value, albumImageId ?? 0, NetworkLink_AlbumImage, data);
                data = null;
                NotifyUpdate("AlbumImage");
                GlobalViewModel.GetInstance().NotifyCoverUpdate(); // In any cases we will refresh the states cover on player dock
            }
            GC.Collect();
        }
        public void NotifyUpdate(string memberName = null)
        {
            OnPropertyChanged(memberName);
        }

        private static BitmapSource GetAlbumIllust(byte[] data)
        {
            if (data is null || data.Length == 0)
                return null;
            try
            {
                BitmapSource bs;
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (Image image = Image.FromStream(stream))
                    {
                        bs = (image as Bitmap).Convert();
                        bs.Freeze(); // It should be called before Binding Image otherwize will throw a ArgumentException about threading error
                                               // Thanks Stackoverflow: https://stackoverflow.com/questions/26361020/error-must-create-dependencysource-on-same-thread-as-the-dependencyobject-even
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return bs;
            }
            catch
            {
                return null;
            }
        }
        public BitmapSource GetCoverCache () => GetAlbumIllust(CacheManager.GetImageCache(songId ??  -1, NetworkLink_AlbumImage, albumImageId));

        /*void GetAlbumIllust(TagLib.IPicture[] pics)
        {
            if (pics.Length >= 1)
            {
                try
                {
                    int index = pics.Length - 1; // Pick the last one
                    //ApplyAlbumIllust(pics[index].Data.Data);
                }
                catch (Exception e)
                {
                    AlbumImage = null;
                    AlbumIllustErrorInfo = e.Message;
                }
            }
        }*/ 
    }
}
