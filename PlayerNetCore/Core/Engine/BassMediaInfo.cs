using System;
using System.Collections.Generic;
using System.Text;
using ManagedBass;

namespace NekoPlayer.Core.Engine
{
    /// <summary>
    /// Provide the infos after loaded playable. For send arguments of event only.
    /// </summary>
    public class BassMediaInfo
    {
        internal BassMediaInfo(IPlayable media, ChannelInfo channelInfo)
        {
            Playable = media;
            ChannelCounts = channelInfo.Channels;
            MediaType = channelInfo.ChannelType;
            PlaybackFrequency = channelInfo.Frequency;
        }
        public IPlayable Playable;
        public int ChannelCounts;
        public ChannelType MediaType;
        public int PlaybackFrequency;
    }
}
