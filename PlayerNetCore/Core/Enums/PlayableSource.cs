using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Core
{
    /// <summary>
    /// Flags for knowing playable source from (Local or online)
    /// </summary>
    public enum PlayableSource
    {
        /// <summary>
        /// Means playable source from local (Hard Disk, USB Storage, Mass Storage or RAM Memory)
        /// </summary>
        Local = 0,
        /// <summary>
        /// Means playable source form online (HTTP protocol, FTP protocol, local network etc.)
        /// </summary>
        Online = 1
    }
}
