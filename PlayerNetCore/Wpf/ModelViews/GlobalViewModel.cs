using NekoPlayer.Core;
using NekoPlayer.Core.Interfaces;
using NekoPlayer.Core.Playlists;
using NekoPlayer.Core.Shuffle;
using NekoPlayer.Wpf.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Windows;
using PlayerNetCore;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace NekoPlayer.Wpf.ModelViews
{
    /// <summary>
    /// A view model that provides now playing playable, all playlists, state player and etc.
    /// </summary>
    public class GlobalViewModel : ViewModelBase, IPlayerContext
    {
        public const int Repeat_NoRepeat = 0;
        public const int Repeat_RepeatPlaylist = 1;
        public const int Repeat_RepeatOnePlayable = 2;

        private readonly static WeakReference<GlobalViewModel> instance = new WeakReference<GlobalViewModel>(null);
        private double m_CurrentPosition;
        private string m_DurationString;
        private int m_Volume = 100;
        public static GlobalViewModel GetInstance()
        {
            GlobalViewModel o = null;
            if (instance.TryGetTarget(out o))
                return o;
            else
                return null;
        }
        public static void Initialize()
        {
            if (instance.TryGetTarget(out var o))
            {
                return;
            }
            instance.SetTarget(new GlobalViewModel());
        }
        /// <summary>
        /// Constructor for creating context
        /// </summary>
        private GlobalViewModel()
        {
            RecentPlaylist = new RecentPlaylist();
            RecentPlaylist.OnPlaylistChanges += (o, e) => OnPropertyChanged(nameof(Playlists));
            Playlists = new ObservableCollection<IPlaylist>();
            Playlists.Add(RecentPlaylist);
            var playlists = SettingsManager.GetContexts(SettingsManager.UserPlaylistsPath);
            foreach (var item in playlists)
            {
                var p = new Playlist(item);
                Playlists.Add(p);
            }
            Duration = 0.0;
            CurrentPosition = 0.0;
            CurrentShuffleAlgorithm = new FisherYatesShuffle();
            PlayableIndex = -1;
            if (!Appleneko2001.Utils.IsXamlDesignerMode())
                LoadSettings();
        }

        public event EventHandler<int> OnVolumeChanged;
        #region Members that WPF can be binded
        public int RepeatMode { get; private set; }

        public string DurationString { get { return m_DurationString; } private set { m_DurationString = value;OnPropertyChanged(); } }
        public double Duration { get; private set; }
        public double CurrentPosition
        {
            get { return m_CurrentPosition; }
            set
            {
                m_CurrentPosition = value;
                OnPropertyChanged();
            }
        }
        public void OnPlayStatementUpdate(object eventSenderSource, double currentPos)
        {
            CurrentPosition = currentPos;
            UpdateDurationString();
        }
        public void UpdateDurationString()
        {
            var suffix = NowPlayingItem?.Duration.ToString("mm\\:ss", CultureInfo.InvariantCulture) ?? TimeSpan.FromSeconds(0.0).ToString("mm\\:ss", CultureInfo.InvariantCulture);
            var prefix = TimeSpan.FromSeconds(CurrentPosition).ToString("mm\\:ss", CultureInfo.InvariantCulture);
            DurationString = $"{prefix} / {suffix}";
        }
        public NowPlayingPlaylist CurrentPlaylist { get; private set; }
        public bool LoadedPlaylist { get { return CurrentPlaylist != null; } }
        public RecentPlaylist RecentPlaylist { get; private set; }
        public IPlayable NowPlayingItem { get; private set; }
        private BitmapSource m_BassNowPlayingCover;
        public BitmapSource NowPlayingCover { 
            get 
            { 
                if(NowPlayingItem != null && NowPlayingItem.TrackInfo != null)
                {
                    var result = NowPlayingItem.TrackInfo.GetCoverCache();
                    if (result != null)
                        return result;
                }
                return m_BassNowPlayingCover;
            } 
        }
        public bool IsMediaLoaded { get; private set; }
        public ObservableCollection<IPlaylist> Playlists { get; private set; }
        public bool IsPlaying { get; private set; }
        public bool IsMuted { get; private set; }
        public bool IsShuffleOn { get; private set; }
        public int Volume { get => m_Volume; 
            set {
                int safe = value;
                safe = (value > 100 ? 100 : value < 0 ? 0 : safe); 
                m_Volume = safe; 
                OnVolumeChanged?.Invoke(this, m_Volume); 
                OnPropertyChanged(); } }
        #endregion
        #region Some member setter with notify context changes.
        public void SetCurrentPlaylist(IPlaylist p)
        {
            CurrentPlaylist = new NowPlayingPlaylist(p);
            GetShuffledCurrentPlaylist(Player.GetRandomizedSeed);
            OnPropertyChanged(nameof(CurrentPlaylist));
            OnPropertyChanged(nameof(LoadedPlaylist));
        }
        public void SetNowPlaying(IPlayable p)
        {
            IsMediaLoaded = p != null;
            OnPropertyChanged(nameof(IsMediaLoaded));
            NowPlayingItem = p;
            RecentPlaylist.AddPlayable(p);
            Duration = p?.Duration.TotalSeconds ?? 0.0;
            DurationString = p?.Duration.ToString("mm\\:ss", CultureInfo.InvariantCulture) ?? 
                TimeSpan.FromSeconds(0.0).ToString("mm\\:ss", CultureInfo.InvariantCulture);
            OnPropertyChanged(nameof(NowPlayingItem));
            OnPropertyChanged(nameof(Duration));
            OnPropertyChanged(nameof(DurationString));
        }
        public void SetCover(BitmapSource cover)
        {
            m_BassNowPlayingCover = cover;
            OnPropertyChanged(nameof(NowPlayingCover));
        }
        public void SetPlayingState(bool b)
        {
            IsPlaying = b;
            OnPropertyChanged(nameof(IsPlaying));
        }
        public void SetMutedState(bool b)
        {
            IsMuted = b;
            OnPropertyChanged(nameof(IsMuted));
        }
        public void SetShuffleState(bool b)
        {
            IsShuffleOn = b;
            OnPropertyChanged(nameof(IsShuffleOn));
        }
        public void SetRepeatState(int mode)
        {
            // if (3 > mode > 0) == true
            if (3 > mode && mode > 0)
                RepeatMode = mode;
            // Otherwize we just close repeat mode.
            else
                RepeatMode = 0;
            OnPropertyChanged(nameof(RepeatMode));
        }

        internal void NotifyCoverUpdate() => OnPropertyChanged(nameof(NowPlayingCover));
        #endregion

        public int PlayableIndex;
        public IShuffle CurrentShuffleAlgorithm;
        public Tuple<IPlayable, bool> GetPreviousTrack()
        {
            if (CurrentPlaylist is null)
                return new Tuple<IPlayable, bool>(null, false);

            IPlayable r = null;
            if (PlayableIndex <= 0)
                PlayableIndex = CurrentPlaylist.Playables.Count;
            PlayableIndex--;

            r = PassShuffleGetTrack(PlayableIndex);
            return new Tuple<IPlayable, bool>(r, r != null);
        }
        public Tuple<IPlayable, bool> GetCurrentTrack()
        {
            if (CurrentPlaylist is null)
                return new Tuple<IPlayable, bool>(null, false);
            IPlayable r = null;
            r = PassShuffleGetTrack(PlayableIndex);
            return new Tuple<IPlayable, bool>(r, r != null);
        }
        public Tuple<IPlayable, bool> GetNextTrack(bool isUser = false)
        {
            if(CurrentPlaylist is null)
                return new Tuple<IPlayable, bool>(null, false);

            IPlayable r = null;
            if (isUser)
            {
                if (PlayableIndex < CurrentPlaylist.Playables.Count)
                    PlayableIndex++;
                if (PlayableIndex >= CurrentPlaylist.Playables.Count)
                    PlayableIndex = 0;
                r = PassShuffleGetTrack(PlayableIndex);
                return new Tuple<IPlayable, bool>(r, r != null);
            }
            else
            {
                if (RepeatMode == Repeat_RepeatPlaylist)
                {
                    if (PlayableIndex < CurrentPlaylist.Playables.Count)
                        PlayableIndex++;
                    if (PlayableIndex >= CurrentPlaylist.Playables.Count)
                        PlayableIndex = 0;
                }
                else if (RepeatMode == Repeat_NoRepeat)
                {
                    if (PlayableIndex < CurrentPlaylist.Playables.Count)
                        PlayableIndex++;
                    else
                        return new Tuple<IPlayable, bool>(null, false);
                }
                else if (RepeatMode == Repeat_RepeatOnePlayable)
                    r = NowPlayingItem;
                if (r is null)
                    r = PassShuffleGetTrack(PlayableIndex);
                return new Tuple<IPlayable, bool>(r, r != null);
            }
        }
        public void GetShuffledCurrentPlaylist(int seed = 0)
        {
            CurrentPlaylist?.Shuffle(CurrentShuffleAlgorithm, seed, PlayableIndex);
        }
        public IPlaylist RequestPlaylist(string id)
        {
            return Playlists.Where(p => p.Identicator == id).FirstOrDefault();
        }
        public IPlaylist CreatePlaylist(string name)
        {
            var playlist = new Playlist(name, name);
            Playlists.Add(playlist);
            return playlist;
        }
        public void DeletePlaylist(string id)
        {
            var playlist = Playlists.Where(p => p.Identicator == id).FirstOrDefault();
            if (playlist.IsRemovable)
            {
                playlist.PreDeletePlaylist();
                Playlists.Remove(playlist);
            }
        }
        private IPlayable PassShuffleGetTrack(int pos) => IsShuffleOn ? CurrentPlaylist.PassShuffleGetTrack(pos) : CurrentPlaylist.GetPlayable(pos);
        private void LoadSettings()
        {
            try
            {
                Volume = Math.Clamp(SettingsManager.GetValue<int>(nameof(Volume), 100), 0, 100);
                SetMutedState(SettingsManager.GetValue<bool>(nameof(IsMuted), false));
                SetShuffleState(SettingsManager.GetValue<bool>(nameof(IsShuffleOn), false));
                SetRepeatState(Math.Clamp(SettingsManager.GetValue<int>(nameof(RepeatMode), 0), 0, 2));
            }
            catch 
            { }
        }
        public void RequestSaveSettings()
        {
            SettingsManager.SetValue(nameof(Volume), Volume);
            SettingsManager.SetValue(nameof(IsMuted), IsMuted);
            SettingsManager.SetValue(nameof(IsShuffleOn), IsShuffleOn);
            SettingsManager.SetValue(nameof(RepeatMode), RepeatMode);
        }
    }
}
