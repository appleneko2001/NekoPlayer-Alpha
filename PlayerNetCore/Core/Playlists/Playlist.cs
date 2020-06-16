using Appleneko2001;
using MaterialDesignThemes.Wpf;
using NekoPlayer.Core.Interfaces;
using NekoPlayer.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NekoPlayer.Core.Playlists
{
    public class PlaylistSerializator
    {
        public PlaylistSerializator()
        {

        }
        public PlaylistSerializator(Playlist playlist)
        {
            if (playlist == null)
                throw new ArgumentNullException(nameof(playlist));
            Name = playlist.Name;
            PlayablePaths = new List<string>();
            foreach(var item in playlist.Playables)
            {
                PlayablePaths.Add(item.GetMediaPath());
            }
        }
        public string Name;
        public List<string> PlayablePaths;
    }
    public class Playlist : PlaylistBase
    {
        private static int counter = 1;
        private string m_Name;
        private string id;
        public Playlist(string id, string name = null)
        {
            this.id = id.ValidateNameAndFixes();
            AlbumsImage.CollectionChanged += (s, e) => CollectionChanged(nameof(AlbumsImage));
            Playables.CollectionChanged += (s, e) => CollectionChanged(nameof(Playables));
            string path = SettingsManager.UserPlaylistsPath + "\\" + id;
            var load = SettingsManager.LoadContext<PlaylistSerializator>(path);
            if (load != null)
            {
                m_Name = load.Name ?? LanguageManager.RequestNode("playlist.newplaylist.header") + " #" + counter++;
                foreach(var item in load.PlayablePaths)
                {
                    AddPlayable(new Playable(item));
                }
            }
            else
            {
                m_Name = name ?? LanguageManager.RequestNode("playlist.newplaylist.header") + " #" + counter++;
                RequestSaveChanges();
            }
        }

        private void CollectionChanged(string CollectionName)
        {
            OnPropertyChanged(CollectionName);
            OnPropertyChanged(nameof(FullDurationString));
        }
        public override string Identicator => id;
        public override string Name => m_Name;
        public override bool IsNameChangeble => true;
        public override bool IsListItemChangeble => true;
        public override bool IsRemovable => true;
        public override void AddPlayable(IPlayable playable)
        {
            if (playable == null)
                return;
            Playables.Add(playable);
            if(AlbumsImage.Count < 1)
                AlbumsImage.Add(playable.TrackInfo?.AlbumImage);
        }
        public override void DeletePlayable(IPlayable playable)
        {
            Playables.Remove(playable);
            if (AlbumsImage.Count < 1 && Playables.Count > 0)
                AlbumsImage.Add(Playables[0].TrackInfo?.AlbumImage);
            //AlbumsImage.RemoveAt(pos);
        }
        public override void PreDeletePlaylist()
        {
            string path =  SettingsManager.UserPlaylistsPath + "\\" + id;
            SettingsManager.DeleteContext(path);
        }
        public override void RequestSaveChanges()
        {
            string path = SettingsManager.UserPlaylistsPath + "\\" + id;
            var data = new PlaylistSerializator(this);
            SettingsManager.SaveContext(path, data);
        }
        public override void RequestSetName(string name)
        {
            m_Name = name;
            OnPropertyChanged(nameof(Name));
        }
    }
}
