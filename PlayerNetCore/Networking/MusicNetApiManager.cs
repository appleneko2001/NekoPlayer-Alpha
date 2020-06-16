using Netease;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NekoPlayer.Networking
{
    public class MusicNetApiManager
    {
        static MusicNetApiManager instance;
        public static MusicNetApiManager GetInstance() => instance;
        public List<IMusicInfoDatabaseApi> AvaliableMusicInfoApi;
        public List<ILyricDatabaseApi> AvaliableLyricApi;
        public static void Initialize()
        {
            if(instance != null)
            {
                return;
            }
            instance = new MusicNetApiManager();
        }
        private MusicNetApiManager()
        {
            AvaliableMusicInfoApi = new List<IMusicInfoDatabaseApi>();
            AvaliableLyricApi = new List<ILyricDatabaseApi>();
            var netease = new NeteaseMusicApi();
            AvaliableMusicInfoApi.Add(netease);
            AvaliableLyricApi.Add(netease);
        }
        public CompositeCollection GetAvaliableApis(bool GetTrackInfoApi = true, bool GetLyricApi = true)
        {
            var result = new CompositeCollection();
            if (GetTrackInfoApi)
            {

            }
            return result;
        }
        public IMusicInfoDatabaseApi GetApi(int index = 0)
        {
            return AvaliableMusicInfoApi[index];
        }
    }
}
