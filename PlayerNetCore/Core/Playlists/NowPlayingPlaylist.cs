using Appleneko2001;
using MaterialDesignThemes.Wpf;
using NekoPlayer.Core.Interfaces;
using NekoPlayer.Globalization;
using NekoPlayer.Wpf.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace NekoPlayer.Core.Playlists
{
    public class NowPlayingPlaylist : PlaylistBase
    {
        private IPlaylist baseList;
        public NowPlayingPlaylist(IPlaylist playlist)
        {
            baseList = playlist;
            m_PlayableList = new ObservableCollection<IPlayable>(baseList?.Playables.ToArray());
            m_ShuffleIndexes = new ObservableCollection<int>();
        }

        public override string Identicator { get { return "\\/|nekoplayer:nowplaying|\\/"; } }
        public override string Name => baseList.Name;
        public override ObservableCollection<IPlayable> Playables => m_PlayableList;
        public override bool IsNameChangeble => baseList.IsNameChangeble;
        public override bool IsListItemChangeble => true;
        public override bool IsRemovable => false;
        public override PackIconKind? DefaultIcon => null;

        public override void AddPlayable(IPlayable playable) {
            baseList.AddPlayable(playable);
            var i = baseList.Playables.IndexOf(playable);
            m_ShuffleIndexes.Add(i);
        }
        public override void DeletePlayable(IPlayable playable) {
            var i = baseList.Playables.IndexOf(playable);
            baseList.DeletePlayable(playable);
            m_ShuffleIndexes.Remove(i);
        }
        public void Shuffle(IShuffle shuffle, int seed = 0, int PlayableIndex = 0)
        {
            var shuffleMask = shuffle?.GetRandomize(Playables?.Count ?? 0, seed);
            var result = Array.IndexOf(shuffleMask, PlayableIndex);
            if (result != -1)
            {
                int firstPosOldValue = shuffleMask[0];
                shuffleMask[0] = shuffleMask[result];
                shuffleMask[result] = firstPosOldValue;
            }
            m_ShuffleIndexes.Clear();
            foreach (var i in shuffleMask)
                m_ShuffleIndexes.Add(i);
        }
        public IPlayable PassShuffleGetTrack(int pos)
        {
            if (m_ShuffleIndexes.Count > pos)
                return GetPlayable(m_ShuffleIndexes[pos]);
            else
                return GetPlayable(pos);
        }
        private readonly ObservableCollection<IPlayable> m_PlayableList;
        private ObservableCollection<int> m_ShuffleIndexes;
    }
}
