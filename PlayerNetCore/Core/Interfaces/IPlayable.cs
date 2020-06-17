using Appleneko2001;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ManagedBass;

namespace NekoPlayer.Core
{
    /// <summary>
    /// Interface for providing access some general function
    /// </summary>
    public interface IPlayable
    {
        public string GetObjectHash();
        /// <summary>
        /// Return BASS_FILEPROCS object for bass audio engine
        /// </summary>
        /// <returns>BASS_FILEPROCS object when stream not null</returns>
        public FileProcedures GetBassStream();
        public string GetMediaPath();
        public PlayableSource GetPlayableSourceFlag();
        public void Close(bool completelyDispose = false);
        public void CheckStatus();
        public bool IsLocalMedia { get; }
        public bool Ready { get; }
        public void SetCorruptedState(bool v);
        public AudioHeaderData MediaFormat { get; }
        public TimeSpan Duration { get; }
        public TagInfo TrackInfo { get; }
        public string Title { get; }
        public string Artist { get; }
        public string FaultReason { get; }
    }
}
