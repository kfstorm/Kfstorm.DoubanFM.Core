namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// The type of next song command
    /// </summary>
    public enum NextCommandType
    {
        /// <summary>
        /// Current song ended
        /// </summary>
        CurrentSongEnded,
        /// <summary>
        /// Skip current song (not ended)
        /// </summary>
        SkipCurrentSong,
        /// <summary>
        /// Ban current song (hate current song)
        /// </summary>
        BanCurrentSong,
    }
}
