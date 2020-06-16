using NekoPlayer.Core;
using NekoPlayer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media.Imaging;

namespace NekoPlayer.Styles.ModelViews
{
    public class AlbumMaskedPlaylistModel : ViewModelBase, IPlaylist
    {
        private IPlaylist origin;
        public AlbumMaskedPlaylistModel(IPlaylist link)
        {
            origin = link;
            OnPropertyChanged();
        }
        public string Identicator => origin.Identicator;

        public string Name => origin.Name;

        public ObservableCollection<IPlayable> Playables => origin.Playables;

        public bool IsNameChangeble => origin.IsNameChangeble;

        public bool IsListItemChangeble => origin.IsListItemChangeble;

        public bool IsRemovable => origin.IsRemovable;

        public ObservableCollection<BitmapSource> AlbumsImage => throw new NotImplementedException();

        public void AddPlayable(IPlayable playable)
        {
            origin.AddPlayable(playable);
        }

        public void DeletePlayable(int pos)
        {
            origin.DeletePlayable(pos);
        }

        public IPlayable GetPlayable(int index)
        {
            return origin.GetPlayable(index);
        }

        public void RequestSaveChanges()
        {
            origin.RequestSaveChanges();
        }

        public void RequestSetName(string name)
        {
            origin.RequestSetName(name);
        }
    }
}
