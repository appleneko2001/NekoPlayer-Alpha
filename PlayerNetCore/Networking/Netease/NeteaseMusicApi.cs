using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using NekoPlayer.Core.Utilities;
using NekoPlayer.Networking;
using NeteaseCloudMusicApi;
using Newtonsoft.Json.Linq;

namespace Netease
{
    public sealed class NeteaseMusicApi : IMusicInfoDatabaseApi, ILyricDatabaseApi, IDisposable
    {
        public static readonly string ProviderName = "Netease Cloud Music API";
        public static readonly string ProviderLink = "";
        CloudMusicApi core;
        public NeteaseMusicApi()
        {
            core = new CloudMusicApi();
        }
        public string ApiName() => ProviderName;

        public byte[] GetAlbumIllustration(string link)
        {
            try
            {
                var client = new System.Net.WebClient();
                var result = client.DownloadData(link);
                client.Dispose();
                return result;
            }
            catch(Exception e)
            {
                ExceptMessage.PopupExcept(e);
            }
            return Array.Empty<byte>();
        }

        public GetMusicInfoResult GetMusicInfo(long songId)
        {
            var Root = GetResult(CloudMusicApiProviders.SongDetail, new Dictionary<string, string>() { { "ids", songId.ToString(CultureInfo.InvariantCulture) } });
            SongDetailDataModel first = Root["songs"].First.ToObject<SongDetailDataModel>();
            return new GetMusicInfoResult()
            {
                Title = first.name,
                Album = first.al.name,
                Artist = first.GetFullArtistsString(),
                Provider = ProviderName,
                ProviderInfoLink = ProviderLink,
                AlbumIllustrationLink = first.al.picUrl,
                Year = first.PublishTime.Year.ToString(CultureInfo.InvariantCulture),
                SongId = first.id,
                AlbumId = first.al.id,
                AlbumIllustId = first.al.pic,
                AlbumArtist = first.al.artist?.name
            };
        }

        public GetMusicInfoResult GetMusicInfo(string trackInfo)
        {
            throw new NotImplementedException();
        }

        public GetMusicLyricResult GetMusicLyric(long songId)
        {
            var Root = GetResult(CloudMusicApiProviders.Lyric, new Dictionary<string, string>() { { "id", songId.ToString(CultureInfo.InvariantCulture) } });
            LyricDetailDataModel data = Root.ToObject<LyricDetailDataModel>();
            return new GetMusicLyricResult()
            {
                Provider = ProviderName,
                ProviderInfoLink = ProviderLink,
                Language = "unknown",
                LanguageTranslated = "zh-CN",
                WithTranslation = data.tlyric.lyric != null,
                LyricLrcData = data.lrc.lyric,
                TranslateLrcData = data.tlyric.lyric,
                LyricProviderUser = data.lyricUser.nickname,
                TranslateProviderUser = data.transUser.nickname
            };
        }

        public GetMusicLyricResult GetMusicLyric(string trackInfo)
        {
            var r = GetSearch(trackInfo).Results.First();
            if (r is null)
                return null;
            return GetMusicLyric(r.SongId);
        }

        public SearchPageResult GetSearch(string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
                throw new ArgumentNullException("Keywords argument cannot be empty!");
            var query = new Dictionary<string, string>() { { "keywords", keywords } };
            var Root = GetResult(CloudMusicApiProviders.Search, query);
            using (var result = Root["result"].ToObject<SearchSongResultModel>())
            {
                if (result != null)
                {
                    if (result.songCount == 0 && result.songs?.Count != 0)
                        if(result.songs != null)
                            result.songCount = result.songs.Count;
                    return result.ToResult() as SearchPageResult;
                }
                else
                    return new SearchPageResult() { FoundCount = 0 };
            }
        }
        public JObject GetResult(CloudMusicApiProvider func, Dictionary<string, string> param)
        {
            JObject data = null;
            data = core.RequestAsync(func, param).Result.Item2;
            if (data == null)
                throw new ArgumentException("Request result cannot be NULL!");
            else
            {
                var Root = data;
                int requestResult = Root["code"].ToObject<int>();
                if (requestResult == 200)
                {
                    return data;
                }
                throw new NotImplementedException($"Request returned exception ({requestResult})");
            }
        }
        public bool RequiredApikey() => false;

        public bool SetApiKey(string key) => true;

        public void Dispose()
        {
            if (core != null)
                core.Dispose();
        }
    }
}
