using Appleneko2001;
using LinqToDB;
using LinqToDB.Tools;
using NekoPlayer.Core.DbModels;
using NekoPlayer.Core.Utilities;
using NekoPlayer.Networking;
using NekoPlayer.Core.Resources; 
using NekoPlayer.Wpf.ItemsControlViews;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Runtime.InteropServices;

namespace NekoPlayer.Containers
{
    /// <summary>
    /// A cache manager that used for push and remove cache link, lyric link, network tags and cover image.
    /// To use this object are required to <see cref="Initialize()"/> first
    /// </summary>
    public static class CacheManager
    {
        public static string CachesPath { get => Path.Combine(SettingsManager.UserAppdataPath, SettingsManager.UserCachesPath); }
        private static DatabaseContainer CacheContainer;
        public static void DeleteCacheFile() => File.Delete(Path.Combine(CachesPath, "caches.db"));
        public static void Initialize()
        {
            var fullPath = Path.Combine(CachesPath, "caches.db");

            try
            {
                if (!File.Exists(fullPath))
                    using(var stream = ResourceManager.ExtractData("Resources.caches_empty.db"))
                    {
                        using (FileStream file = File.Create(fullPath))
                        {
                            stream.CopyTo(file);
                        }
                    }
                CacheContainer = new DatabaseContainer(CachesPath, "caches.db");
            }
            catch(Exception e)
            {
                ExceptMessage.PopupExcept(e);
            }
        }
        /// <summary>
        /// Push or update the tag cache from internet
        /// </summary>
        /// <param name="trackInfo"></param>
        /// <returns></returns>
        public static bool PushNetworkTagCache(GetMusicInfoResult trackInfo)
        {
            if (trackInfo is null)
                return false;
            try
            {
                var t = CacheContainer.GetTable<NetworkTagsTable>();
                t.Value(v => v.Id, trackInfo.SongId).Value(v => v.Title, trackInfo.Title).
                Value(v => v.Artist, trackInfo.Artist).Value(v => v.Album, trackInfo.Album).
                Value(v => v.ArtistAlbum, trackInfo.AlbumArtist).Value(v => v.TagProvider, trackInfo.Provider).
                Value(v => v.Year, trackInfo.Year).Value(v => v.AlbumImageLink, trackInfo.AlbumIllustrationLink).Insert();

                return true;
            }
            catch (IOException e)
            {
                ExceptMessage.PopupExcept(e);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static ProcessedSegment GetStatusSegmentData()
        {
            long cacheSize = GetCacheFileBytes();
            return new ProcessedSegment(new SegmentTemplate[]
            {
                new SegmentTemplate("Cache database", Color.Aqua, cacheSize),
                //new SegmentTemplate("Cover cache (Online)", Color.Violet, coverSize),
            });
        }
        internal static long GetCacheFileBytes()
        {
            return new FileInfo(Path.Combine(CachesPath, "caches.db")).Length;
        }

        public static bool PushImageCache(long id, long picId, string albumImageLink, byte[] data)
        {
            if (albumImageLink is null || data is null || data.Length is 0)
                return false;
            try
            {
                var ic = CacheContainer.GetTable<ImageCacheTable>();
                ic.Value(v => v.Id, id).Value(v => v.PicId, picId).Value(v => v.AlbumImageLink, albumImageLink).
                    Value(v => v.Data, data).Insert();
                var ntc = CacheContainer.GetTable<NetworkTagsTable>();
                var search = ntc.Where(v => v.Id == id);
                if (search != null)
                {
                    search.Select(v => v.AlbumImageId).Set(null, picId);
                }
                return true;
            }
            catch (IOException e)
            {
                ExceptMessage.PopupExcept(e);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Get image cache by song id
        /// </summary>
        /// <param name="id">song id</param>
        /// <returns>Cache data, return empty array if not found.</returns>
        public static byte[] GetImageCache(long id, string link = null, long? picId = null)
        {
            if (id is -1)
                return Array.Empty<byte>();
            try
            {
                var ic = CacheContainer.GetTable<ImageCacheTable>();
                var result = ic.Where(v => v.Id == id);
                if (result is null)
                    result = ic.Where(v => v.AlbumImageLink == link);
                if (result is null || !result.Any())
                {
                    if(picId != null && picId != 0)
                        result = ic.Where(v => v.PicId == picId);
                }
                if (result != null && result.Any())
                    return result.Select(v => v.Data).ToArray()[0];

            }
            catch(Exception e)
            {
                ExceptMessage.PrintConsole(e);
            }
            return Array.Empty<byte>();
        }
        /// <summary>
        /// Insert a cache link for restore status of online source tags.
        /// Or otherwize we overwrite it instead if it exists.
        /// </summary>
        /// <param name="hash">File hash</param>
        /// <param name="id">song id</param>
        /// <returns></returns>
        public static bool LinkCache(string hash, long id)
        {
            if (hash is null)
                return false;
            try
            {
                var linktable = CacheContainer.GetTable<CacheLinkTable>();
                linktable.InsertOrUpdate(() => new CacheLinkTable { Id = id, Hash = hash,}, t => new CacheLinkTable { Id = id, Hash = t.Hash }) ;
                return true;
            }
            catch
            {

            }
            return false;
        }
        public static bool ContainLink(string hash)
        {
            if (hash is null)
                return false;
            var lt = CacheContainer.GetTable<CacheLinkTable>();
            var result = lt.Where(v => v.Hash == hash);
            if (result != null && result.Any())
                return true;
            return false;
        }
        public static void DownsizeCover()
        {
            var t = CacheContainer.GetTable<ImageCacheTable>();

            int index = 0, count = t.Count() ;
            while (index < count)
            {
                var item = t.ElementAt(index);
                try
                {
                    var resized = Utils.ResizeBitmap(item.Data, 1000);
                    t.Where(v => v.Id == item.Id)
                        .Where(v => v.PicId == item.PicId)
                        .Set(v => v.Data, resized)
                        .Update();
                    resized = null;
                    GC.Collect();
                }
                catch (Exception e)
                {
                    ExceptMessage.PopupExcept(e, false, $"Track Id = {item.Id}");
                }
                item.Data = null;
                item.AlbumImageLink = null;
                item.Id = 0;
                item.PicId = 0;
                item = null;
                index++;
            }
            GC.Collect();
        }
        public static long GetLinkCache(string hash)
        {
            if (hash is null)
                throw new ArgumentNullException(nameof(hash));
            var lt = CacheContainer.GetTable<CacheLinkTable>();
            var result = lt.Where(v => v.Hash == hash);
            if (result != null && result.Any())
                return result.Select(v => v.Id).ToArray()[0];
            throw new KeyNotFoundException();
        }
        public static NetworkTagsTable GetNetworkTagCache(long id)
        {
            var t = CacheContainer.GetTable<NetworkTagsTable>();
            var result = t.Where(v => v.Id == id);
            if (result != null && result.Any())
                return result.ToArray()[0];
            throw new KeyNotFoundException();
        }
        public static void HashLinkCorrection()
        {
            var lt = CacheContainer.GetTable<CacheLinkTable>();
            var result = lt.ToArray();
            foreach(var item in result)
            {
                lt.Where(v => v.Id == item.Id)
                    .Set(v => v.Hash, item.Hash.Replace("-", "", StringComparison.Ordinal))
                    .Update();
            }
            List<string> foundHashs = new List<string>();
            foreach (var item in result)
            {
                if (!foundHashs.Contains(item.Hash))
                    foundHashs.Add(item.Hash);
            }
            // Remove all expect last one
            foreach(var hash in foundHashs)
            {
                var founds = lt.Where(v => v.Hash == hash);
                while ((founds = lt.Where(v => v.Hash == hash)).Count() > 1)
                {
                    var first = founds.First();
                    founds.Delete(predicate => predicate == first);
                }
            }
        }
        public static bool LinkLyric (string hash, string toFile)
        {
            if (hash is null)
                return false;
            try
            {
                var linktable = CacheContainer.GetTable<LyricLinkTable>();
                linktable.InsertOrUpdate(() => new LyricLinkTable { Hash = hash, ToFile = toFile }, t => new LyricLinkTable { Hash = hash, ToFile = toFile });
                return true;
            }
            catch
            {

            }
            return false;
        }
    }
}
