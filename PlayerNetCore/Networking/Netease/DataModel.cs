// Updated time: 07.03.2020
// version 1 
// Implemented Search result, track details, album and artist details.
// Lyrics support is planning.
using NekoPlayer.Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netease
{
    public class ResultDataModel
    {
        public object result;
        public int code;
    }
    public sealed class SearchSongResultModel : IResult, IDisposable
    {
        private bool disposed;
        public List<SongItemDataModel> songs;
        public int songCount;

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                if(songs != null)
                songs.Clear();
                songs = null;
                songCount = 0;
                GC.Collect();
            }
        }

        public T ToResult<T>()
        {
            throw new NotImplementedException();
        }

        public object ToResult()
        {
            SearchPageResult result = new SearchPageResult();
            result.FoundCount = songCount;
            if(songs != null)
            foreach (var item in songs)
            {
                string album = item.album.name + (item.album.trans == null ? "" : $" ({item.album.trans})");
                result.Results.Add(new SearchResult(item.id, NeteaseMusicApi.ProviderName, NeteaseMusicApi.ProviderLink, item.name,
                    item.GetFullArtistsString(), album, false));
            }
            return result;
        }
    }
    public class SongItemDataModel
    {
        public int id;
        public string name;
        public int copyrightId;
        public int status;
        public List<ArtistInfoDataModel> artists;
        public AlbumInfoDataModel album;
        public string GetFullArtistsString()
        {
            string result = "";
            int seperators = artists.Count - 1;
            for(int i = 0;i < artists.Count; i++)
            {
                result += artists[i].name + (artists[i].trans == null ? "" : $"({artists[i].trans})");
                if (seperators > 0)
                {
                    result+= ", ";
                    seperators--;
                }
            }
            return result;
        }
    }
    public sealed class SongDetailDataModel : IDisposable
    {
        private bool disposed;
        public string name; // Track name
        public int id; // Database item id
        public int no; // Track number
        public List<ArtistInfoDataModel> ar; // Artists
        public AlbumInfoDataModel al; // Album
        public long publishTime;
        public DateTime PublishTime => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(publishTime).ToLocalTime(); // From milisecond long time stamp to DateTime

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                name = null;
                id = 0;
                no = 0;
                ar.Clear();
                ar = null;
                al = null;
                publishTime = 0;
                GC.Collect();
            }
        }

        public string GetFullArtistsString()
        {
            string result = "";
            int seperators = ar.Count - 1;
            for (int i = 0; i < ar.Count; i++)
            {
                result += ar[i].name + (ar[i].trans == null ? "" : $"({ar[i].trans})");
                if (seperators > 0)
                {
                    result += ", ";
                    seperators--;
                }
            }
            return result;
        }
    }
    public class ArtistInfoDataModel
    {
        public int id;
        public string name;
        public string trans; // Translated
    }
    public class AlbumInfoDataModel
    {
        public int id;
        public string name;
        public string picId;
        public string picUrl; // Album picture link
        public long pic;
        public string trans; // Translated
        public ArtistInfoDataModel artist;
    }
    public class LyricDetailDataModel
    {
        public bool sgc;
        public bool sfy;
        public bool qfy;
        public UserInfoDataModel transUser; // Translation provider user
        public UserInfoDataModel lyricUser; // Original lyric provider user
        public LrcDataModel lrc; // Original lyric data
        public LrcDataModel klyric; // Real-Time lyric data
        public LrcDataModel tlyric; // Translated lyric data
    }
    public class UserInfoDataModel
    {
        public int id;
        public string nickname;
    }
    public class LrcDataModel
    {
        public int version;
        public string lyric;
    }
}
