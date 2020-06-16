#nullable enable
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media.Imaging;

namespace NekoPlayer.Core.Interfaces
{
    public interface IPlaylist
    {
        public string FullDurationString { get; }
        public string Identicator { get; }
        public string Name { get; }
        public ObservableCollection<IPlayable> Playables { get; }
        public ObservableCollection<BitmapSource> AlbumsImage { get; }
        public PackIconKind? DefaultIcon { get; }
        public bool IsNameChangeble { get; }
        public bool IsListItemChangeble { get; }
        public bool IsRemovable { get; }
        public void AddPlayable(IPlayable playable);
        public void DeletePlayable(IPlayable playable);
        public IPlayable GetPlayable(int index);
        public void PreDeletePlaylist();
        public void RequestSetName(string name);
        public void RequestSaveChanges();
        public int GetTrackCount();
    }
}
