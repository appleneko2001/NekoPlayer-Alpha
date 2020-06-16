namespace NekoPlayer.Core
{
    /// <summary>
    /// Interface for implements <see cref="BassFileStream"/> Capsulation object
    /// </summary>
    public interface IBassFileProcs
    {
        /// <summary>
        /// Request a BASS_FILEPROCS object (<see cref="BassFileStream"/> in ManagedBass)
        /// </summary>
        /// <returns>just BASS_FILEPROCS, nothing else otherwize you trying to get it from disposed object, it is impossible!</returns>
        public BassFileStream GetBassFileController();
    }
}
