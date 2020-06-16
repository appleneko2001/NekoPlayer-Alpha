using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Core.Engine
{
    /// <summary>
    /// Implements encapsulating of BASS device
    /// </summary>
    public interface IBassDevice
    {
        /// <summary>
        /// The audio device name.
        /// </summary>
        public string DeviceName { get; }
        /// <summary>
        /// Device driver identicator.
        /// </summary>
        public string DeviceId { get; }
        /// <summary>
        /// Defines device type (Headset, Microphone or etc.)
        /// </summary>
        public string DeviceType { get; }
        /// <summary>
        /// Identicator in BASS audio library. It should providen from BASS host.
        /// </summary>
        public int Identicator { get; }
    }
}
