using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Core.Engine
{
    public sealed class BassChannelStatus
    {
        public int StreamId { get; private set; }
        public BassChannelStatus(int id)
        {
            StreamId = id;
        }
        public void Dispose()
        {
            StreamId = 0;
        }
    }
}
