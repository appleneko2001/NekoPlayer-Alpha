using Appleneko2001;
using MaterialDesignThemes.Wpf;
using NekoPlayer.Core.Interfaces;
using NekoPlayer.Globalization;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace NekoPlayer.Core.Playlists
{
    /// <summary>
    /// An base class of Playlist, provides some completed codes but all are not implemented state.
    /// You need use this base to create an another type playlist.
    /// </summary>
    public abstract class PlaylistBase : ViewModelBase, IPlaylist
    {
        public PlaylistBase()
        {
            m_Playables = new ObservableCollection<IPlayable>();
            m_AlbumsImage = new ObservableCollection<BitmapSource>();
        }

        private string m_FullDurationString;
        private ObservableCollection<IPlayable> m_Playables;
        private ObservableCollection<BitmapSource> m_AlbumsImage;

        private void UpdateFullDurationString()
        {
            var durationStr = Playables.GetFullDuration() ?? "";
            if (!durationStr.StartsWith("#!", StringComparison.OrdinalIgnoreCase))
                m_FullDurationString = LanguageManager.Combine("#common.duration", ": ", durationStr);
            else
                m_FullDurationString = LanguageManager.Combine("#common.error", ": ", durationStr.Substring(2));
        }
        /// <summary>
        /// Returns durations of all playables.
        /// Completed code. If any reasons you can rewrite it.
        /// </summary>
        public virtual string FullDurationString
        {
            get { UpdateFullDurationString(); return m_FullDurationString; }
        }
        /// <summary>
        /// Always return null.
        /// Override are required.
        /// </summary>
        public virtual string Identicator => null;
        /// <summary>
        /// Always return null.
        /// Override are required.
        /// </summary>
        public virtual string Name => null;
        public virtual ObservableCollection<IPlayable> Playables => m_Playables;
        public virtual ObservableCollection<BitmapSource> AlbumsImage => m_AlbumsImage;
        /// <summary>
        /// A icon will show if no any cover found in AlbumsImage.
        /// Always returned PlaylistStar.
        /// Availables icon kinds can founded in Pack Icon list from MaterialDesignInXAML Demo.
        /// </summary>
        public virtual PackIconKind? DefaultIcon => PackIconKind.PlaylistStar;
        /// <summary>
        /// Always returns false. Use override to return other value.
        /// </summary>
        public virtual bool IsNameChangeble => false;
        /// <summary>
        /// Always returns false. Use override to return other value.
        /// </summary>
        public virtual bool IsListItemChangeble => false;
        /// <summary>
        /// Always returns false. Use override to return other value.
        /// </summary>
        public virtual bool IsRemovable => false;
        /// <summary>
        /// It actually a empty method.
        /// Use override to rewrite the method for implements.
        /// </summary>
        /// <param name="playable">Requested for adding new playable to list.</param>
        public virtual void AddPlayable(IPlayable playable) { }
        /// <summary>
        /// It actually a empty method.
        /// Use override to rewrite the method for implements.
        /// </summary>
        /// <param name="playable">Requested for removing playable to list.</param>
        public virtual void DeletePlayable(IPlayable playable) { }
        /// <summary>
        /// This method used for get playable from list (Without shuffle support).
        /// </summary>
        /// <param name="index">Number of object.</param>
        /// <returns>Playable object or null (if nothing in playlist).</returns>
        public virtual IPlayable GetPlayable(int index)
        {
            var r = (Playables.Count >= 1 && index < Playables.Count) ? Playables[index] : null;
            return r;
        }
        public virtual int GetTrackCount() => Playables.Count;
        /// <summary>
        /// It actually a empty method.
        /// This method will called before accepted deleting playlist.
        /// </summary>
        public virtual void PreDeletePlaylist() { }
        /// <summary>
        /// It actually a empty method.
        /// This method will called after requested save all changes.
        /// </summary>
        public virtual void RequestSaveChanges() { }
        /// <summary>
        /// It actually a empty method.
        /// This method will called after requested to changes playlist name.
        /// </summary>
        public virtual void RequestSetName(string name) { }
    }
}
