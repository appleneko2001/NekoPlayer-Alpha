using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using NekoPlayer.Core;
using NekoPlayer.Core.Utilities;
using NekoPlayer.Globalization;
using NekoPlayer.Networking;
using NekoPlayer.Wpf.Commands;
using NekoPlayer.Wpf.ModelViews;

namespace NekoPlayer.Wpf.Dialogs
{
    public class GetTrackInfoViewModel : ViewModelBase
    {
        public bool GetTrackInfo, GetLyrics;
        public GetMusicInfoResult FinalResult;
        public SearchResult SelectedSearchItem;
        private IPlayable playable;
        public List<IMusicInfoDatabaseApi> AvaliableApis;
        private ObservableCollection<string> m_AvaliableApisString ;
        public ObservableCollection<string> AvaliableApisString => m_AvaliableApisString;
        public GetTrackInfoViewModel(IPlayable playable, bool GetTrackInfo = true, bool GetLyrics = true)
        {
            if (playable is null)
                throw new ArgumentNullException(nameof(playable));
            m_AvaliableApisString = new ObservableCollection<string>();
            var apiMan = MusicNetApiManager.GetInstance();
            AvaliableApis = apiMan.AvaliableMusicInfoApi;
            foreach(var item in AvaliableApis)
            {
                AvaliableApisString.Add(item.ApiName());
            }
            OnPropertyChanged("AvaliableApisString");
            this.playable = playable;
            this.GetTrackInfo = GetTrackInfo;
            this.GetLyrics = GetLyrics;
            Keywords = "";
            Keywords += playable.TrackInfo.Artist + ' ';
            Keywords += playable.TrackInfo.Album + ' ';
            Keywords += playable.TrackInfo.Title;
        }
        private SearchPageResult m_SearchResult;
        public SearchPageResult SearchResult { get { return m_SearchResult; } set { m_SearchResult = value;
                OnPropertyChanged();
            } }
        private string m_MessageText;
        public string MessageText { get { return m_MessageText; } set { m_MessageText = value;OnPropertyChanged(); } }

        private string m_Keywords;
        public string Keywords { get { return m_Keywords; } set { m_Keywords = value; OnPropertyChanged(); } }

        #region For commands
        bool m_Working;
        public bool Working { get { return m_Working; } set { m_Working = value;OnPropertyChanged(); } }
        internal IMusicInfoDatabaseApi SelectedApi;
        private int m_SelectedResult = -1;
        public int SelectedResult { get { return m_SelectedResult; } 
            set 
            { 
                m_SelectedResult = value;
                OnPropertyChanged();
                if (m_SelectedResult >= 0)
                {
                    var item = SearchResult.Results[m_SelectedResult];
                    MessageText = LanguageManager.RequestNode("common.selected") + $"{item.Artist} - {item.Album} - {item.Title}";
                    SelectedSearchItem = item;
                }
                else
                {
                    MessageText = null;
                    SelectedSearchItem = null;
                }
            } 
        }
        public ICommand StartSearchCommand => new RelayCommand(StartSearch, StartSearch_CanExecute);
        //public ICommand ReturnResultCommand => new CommandModel(ReturnResult, ReturnResult_CanExecute);
        public ICommand ReturnResultCommand => new RelayCommand(ReturnResult, ReturnResult_CanExecute);
        private void StartSearch(object o)
        {
            Task.Run(() =>
            {
                Working = true;
                try
                {
                    if (SelectedApi == null)
                        return;
                    MessageText = LanguageManager.RequestNode("common.searching");
                    var result = SelectedApi.GetSearch(Keywords);
                    if (result != null)
                    {
                        SearchResult = result;
                        MessageText = LanguageManager.RequestNode("common.result") + result.FoundCount;
                        OnPropertyChanged(nameof(SearchResult));
                    }
                }
                catch(Exception e)
                {
                    ExceptMessage.PrintConsole(e);
                    MessageText = LanguageManager.RequestNode("error.working.header") + e.Message;
                }
                Working = false;
            });
        }
        private bool StartSearch_CanExecute(object o)
        {
            if (SelectedApi == null || string.IsNullOrWhiteSpace(Keywords) || Working)
                return false;
            return true;
        }

        private void ReturnResult(object o)
        {
            Task.Run(() =>
            {
                Working = true;
                if (SelectedSearchItem == null)
                    return;
                MessageText = LanguageManager.RequestNode("dialog.gettrackinfo.working");
                try
                {
                    if (GetTrackInfo)
                    {
                        FinalResult = SelectedApi.GetMusicInfo(SelectedSearchItem.SongId);
                        playable.TrackInfo.ApplyTagInfo(FinalResult);
                        playable.TrackInfo.DownloadAlbumIllust();
                    }
                    if (GetLyrics)
                    {

                    }
                    var obj = o as Control;

                    obj.Dispatcher.Invoke(() =>
                    DialogHost.CloseDialogCommand.Execute(obj.Tag, obj));
                }
                catch(Exception e)
                {
                    ExceptMessage.PrintConsole(e);
                    MessageText = LanguageManager.RequestNode("error.working.header") + e.Message;
                    Working = false;
                }
            });
        }
        private bool ReturnResult_CanExecute(object o)
        {
            if (Working || SelectedSearchItem == null)
                return false;
            return true;
        }
        #endregion
        public void Dispose()
        {
            this.Keywords = null;
            this.FinalResult = null;
            this.MessageText = null;
            this.SearchResult = null;
            this.SelectedSearchItem = null;
            this.Working = false;
            this.AvaliableApisString.Clear();
        }
    }
    public partial class GetTrackInfoDialog : UserControl
    {
        GetTrackInfoViewModel mvvm { get; set; }
        public GetTrackInfoDialog(IPlayable playable, bool GetTrackInfo = true, bool GetLyrics = true)
        {
            InitializeComponent();
            mvvm = new GetTrackInfoViewModel(playable, GetTrackInfo, GetLyrics);
            DataContext = mvvm;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var SelectedIndex = comboBox.SelectedIndex;
            mvvm.SelectedApi = SelectedIndex >= 0 ? mvvm.AvaliableApis[SelectedIndex] : null;
        }
        public GetMusicInfoResult GetResult() => mvvm.FinalResult;
        public void Dispose()
        {
            mvvm.Dispose();
            DataContext = null;
            GC.SuppressFinalize(mvvm);
            mvvm = null;
            GC.Collect();
        }
    }
}
