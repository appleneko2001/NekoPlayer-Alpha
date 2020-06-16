namespace NekoPlayer.Core.Interfaces
{
    /// <summary>
    /// Shuffle algorithms interface.
    /// Reference this interface are required if you want to implement an another algorithms.
    /// </summary>
    public interface IShuffle
    {
        /// <summary>
        /// Get indexes of shuffled list
        /// </summary>
        /// <param name="count">Track counts</param>
        /// <param name="seed">Random seed</param>
        /// <returns></returns>
        public int[] GetRandomize(int count, int seed = 0);
    }
}
