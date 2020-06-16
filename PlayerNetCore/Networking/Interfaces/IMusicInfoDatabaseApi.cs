using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NekoPlayer.Networking
{
    public interface IMusicInfoDatabaseApi
    {
        string ApiName();
        bool RequiredApikey();
        bool SetApiKey(string key);
        SearchPageResult GetSearch(string keywords = "");
        GetMusicInfoResult GetMusicInfo(long songId);
        GetMusicInfoResult GetMusicInfo(string trackInfo);
        byte[] GetAlbumIllustration(string link);
    }
}
