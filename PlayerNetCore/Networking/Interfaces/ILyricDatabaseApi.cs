using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Networking
{
    public interface ILyricDatabaseApi
    {
        string ApiName();
        bool RequiredApikey();
        bool SetApiKey(string key);
        SearchPageResult GetSearch(string keywords = "");
        GetMusicLyricResult GetMusicLyric(long songId);
        GetMusicLyricResult GetMusicLyric(string trackInfo);
    }
}
