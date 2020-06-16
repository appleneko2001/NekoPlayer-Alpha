using NekoPlayer.Core;
using NekoPlayer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NekoPlayer.Wpf.Interfaces
{
    public interface IPlayerContext
    {
        public IPlayable NowPlayingItem { get; }
        public ObservableCollection<IPlaylist> Playlists { get; }
        public bool IsPlaying { get; }
        public string DurationString { get; }
        public double Duration { get; }
        public double CurrentPosition { get; set; }
        public int Volume { get; set; }
    }
}
