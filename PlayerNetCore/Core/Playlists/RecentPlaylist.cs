using NekoPlayer.Core.Interfaces;
using NekoPlayer.Globalization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using System.Globalization;
using Appleneko2001;

namespace NekoPlayer.Core.Playlists
{
    public class RecentPlaylist : PlaylistBase
    {
        public Dispatcher Thread;
        public event EventHandler OnPlaylistChanges;
        public RecentPlaylist()
        {
            Thread = Dispatcher.CurrentDispatcher;
            AlbumsImage.CollectionChanged += (s, e) => CollectionChanged(nameof(AlbumsImage));
            //AlbumsImage.Add(new BitmapImage(new Uri(PlayerNetCore.App.RecentImageUri)));
            Playables.CollectionChanged += (s, e) => CollectionChanged(nameof(Playables));
            var load = SettingsManager.LoadContext<List<string>>(SettingsManager.UserRecentFilePath);
            if (load != null)
            {
                foreach (var item in load)
                {
                    var p = new Playable(item);
                    AddPlayable_Internal(p, true);
                }
            }
            else
            {
                RequestSaveChanges();
            }
        }
        private void CollectionChanged(string CollectionName)
        {
            OnPropertyChanged(CollectionName);
            OnPlaylistChanges?.Invoke(this, null);
            OnPropertyChanged(nameof(FullDurationString));
        }
        public override string Identicator { get { return "|____recents|"; } }
        public override string Name => LanguageManager.RequestNode("playlist.recent");
        public override bool IsNameChangeble => false;
        public override bool IsListItemChangeble => true;
        public override bool IsRemovable => false;

        public override PackIconKind? DefaultIcon => PackIconKind.PlaylistNote;

        /// <summary>
        /// Join a playable to recent playlist, or resort items in playlist.
        /// </summary>
        /// <param name="playable"></param>
        public override void AddPlayable(IPlayable playable) => AddPlayable_Internal(playable, false);
        private void AddPlayable_Internal(IPlayable playable, bool fromInit)
        {
            var result = Playables.Where(o =>
    o.GetMediaPath() == playable.GetMediaPath() ?
    o.GetObjectHash() == playable.GetObjectHash() : false
);
            if (!result.Any())
            {
                if (!fromInit)
                    Playables.Insert(0, playable);
                else
                    Playables.Add(playable);
            }
            else
            {
                int index = 0;
                List<int[]> moveSequences = new List<int[]>();
                foreach (var item in result)
                {
                    if (Playables.Contains(item))
                    {
                        moveSequences.Add(new int[2] { Playables.IndexOf(item), index });
                        index++;
                    }
                }
                foreach (var item in moveSequences)
                {
                    Thread.Invoke(() => Playables.Move(item[0], item[1]));
                }
                moveSequences.Clear();
            }

        }
        public override void DeletePlayable(IPlayable playable)
        {
            Playables.Remove(playable);
            RequestSaveChanges();
        }
        public override void RequestSaveChanges()
        {
            List<string> list = new List<string>();
            foreach (var item in Playables)
            {
                list.Add(item.GetMediaPath());
            }
            SettingsManager.SaveContext(SettingsManager.UserRecentFilePath, list);
        }
    }
}
