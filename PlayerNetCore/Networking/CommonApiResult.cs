using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NekoPlayer.Networking
{
    public class SearchPageResult : ViewModelBase //For binding support, we added a base DataContextViewModelClass for use command OnPropertyUpdate
    {
        public SearchPageResult()
        {
            m_Results = new ObservableCollection<SearchResult>();
        }
        private ObservableCollection<SearchResult> m_Results;
        public ObservableCollection<SearchResult> Results => m_Results;
        private int m_FoundCount;
        public int FoundCount { get { return m_FoundCount; } set { m_FoundCount = value; OnPropertyChanged(); } }
        public int Page;
    }
    public class SearchResult : ViewModelBase
    {
        public SearchResult(long songId, string provider, string providerLink = "", string title = "", string artist = "", string album = "", bool restricted=false)
        {
            SongId = songId;
            Provider = provider;
            ProviderInfoLink = providerLink;
            Title = title;
            Artist = artist;
            Album = album;
            Restricted = restricted;
            Update();
        }
        public string Provider { get; private set; }
        public string ProviderInfoLink { get; private set; }
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public string Album { get; private set; }
        public long SongId { get; private set; }
        public bool Restricted { get; private set; }
        public void Update()
        {
            OnPropertyChanged();
        }
    }
    public class GetMusicInfoResult
    {
        public string Provider;
        public string ProviderInfoLink;
        public string AlbumIllustrationLink;
        public string Title;
        public string Artist;
        public string Album;
        public string AlbumArtist;
        public string Year;
        public string OtherInfo;
        public long? SongId;
        public long? AlbumId;
        public long? AlbumIllustId;
    }
    public class GetMusicLyricResult
    {
        public string Provider;
        public string ProviderInfoLink;
        public bool CommunityProvided;
        public bool WithTranslation;
        public string LyricProviderUser;
        public string TranslateProviderUser;
        public string Language;
        public string LanguageTranslated;
        public string LyricLrcData;
        public string TranslateLrcData;
        //public List<LyricItem> Content;
    }
}
