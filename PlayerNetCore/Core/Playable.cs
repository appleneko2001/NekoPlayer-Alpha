using Appleneko2001;
using ManagedBass;
using NekoPlayer.Core.Utilities;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls.Primitives;

#pragma warning disable CA1031
namespace NekoPlayer.Core
{
    /// <summary>
    /// Local storage playable object
    /// </summary>
    public sealed class Playable : ViewModelBase, IPlayable
    {
        #region Constructor
        /// <summary>
        /// Create object and initializes stream for reading file
        /// </summary>
        /// <param name="filePath">Target file source, it should be existed otherwize will return Exception.</param>
        public Playable(string filePath)
        {
            path = filePath;
            Load();
        }
        #endregion
        public static bool TryCreate(string filePath, out Playable result)
        {
            var header = Analyzer.Analyze(filePath);
            if (header == AudioHeaderData.Unsupported || header == AudioHeaderData.Error)
            {
                result = null;
                return false;
            }
            result = new Playable(filePath);
            return true;
        }
        public bool Load()
        {
            try
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException(string.Format("The media file {0} is not available or it doesn't exist", path), path);
                MediaFormat = Analyzer.Analyze(path);
                if (MediaFormat == AudioHeaderData.Unsupported)
                    throw new FileFormatException(new Uri(path), string.Format("File {0} is not supported temporately", path));
                else if (MediaFormat == AudioHeaderData.Error)
                    throw Analyzer.GetLastError();
                using (stream = new BassFileStream(path))
                {
                    hash = Appleneko2001.Utils.CalculateMurmur3Hash(stream);
                    using (TagLib.File taglib = TagLib.File.Create(stream, Analyzer.GetMimetypeString(MediaFormat), TagLib.ReadStyle.Average))
                    {
                        TrackInfo = new TagInfo(taglib.Tag, this);
                        Duration = taglib.Properties.Duration;
                    }
                }
                if (string.IsNullOrEmpty(TrackInfo.Title)) // Take the file name instead if we can't get the title info from tags
                {
                    TrackInfo.ApplyTitle(Path.GetFileNameWithoutExtension(path));
                }
                Ready = true;
                return true;
            }
            catch (Exception e)
            {
                FaultReason = e.Message;
                ExceptMessage.PrintConsole(3, $"An error occurred when loading media: {e.Message}\r\n{e.StackTrace}\r\nMedia will unavailable until media is returned to online state (Is ready to playback).");
            }
            Ready = false;
            return false;
        }
        public BassFileStream GetStream()
        {
            if (stream == null || !stream.IsDisposed())
            {
                stream = new BassFileStream(path);
                if (!stream.CanRead)
                    throw new ArgumentNullException(null, "Stream not readable, cannot provide stream to read media.");
            }
            return stream;
        }
        #region Getter
        public string GetMediaPath() => path;
        public string GetObjectHash() => hash;
        public FileProcedures GetBassStream() => GetStream().GetBassFileController();
        public PlayableSource GetPlayableSourceFlag() => PlayableSource.Local;
        public bool IsLocalMedia => File.Exists(GetMediaPath());
        public string Title { get { return TrackInfo?.Title ?? ToString(); } }
        public string Artist { get { return TrackInfo?.Artist ?? FaultReason; } }
        #endregion
        public void Close(bool completelyDispose = false)
        {
            if (disposed)
                return;
            if (completelyDispose)
                Dispose();
            else
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
            }
        }
        public void Dispose()
        {
            if (disposed)
                return;
            else
                disposed = true;
            if (stream != null)
                stream.Close();
            stream = null;
            MediaFormat = AudioHeaderData.Unsupported;
            Duration = TimeSpan.Zero;
            TrackInfo = null;
            path = null;
            hash = null;
        }
        #region Public properties
        public AudioHeaderData MediaFormat { get; private set; }
        public TimeSpan Duration { get; private set; }
        private string m_DurationString;
        public string DurationString
        {
            get
            {
                m_DurationString = Duration.ToString("mm\\:ss", CultureInfo.InvariantCulture) ?? "??:??";
                return m_DurationString;
            }
        }
        public TagInfo TrackInfo { get; private set; }
        public bool Ready { get; private set; }
         
        public string FaultReason { get; private set; }
        #endregion
        #region Private variables
        private bool disposed = false;
        private BassFileStream stream;
        private string hash;
        private string path;
        #endregion

        public override string ToString()
        {
            if (TrackInfo is null)
                return $"{path}";
            return $"Track: {TrackInfo.Title} ({path})";
        }

        public void SetCorruptedState(bool v)
        {
            Ready = !v;
            OnPropertyChanged(nameof(Ready));
        }

        public void CheckStatus()
        {
            Load();
        }
    }
}
#pragma warning restore CA1031