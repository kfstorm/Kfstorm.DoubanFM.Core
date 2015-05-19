namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// The type of report
    /// </summary>
    public enum ReportType
    {
        /// <summary>
        /// Current channel changed
        /// </summary>
        CurrentChannelChanged,
        /// <summary>
        /// Skip current song
        /// </summary>
        SkipCurrentSong,
        /// <summary>
        /// Ban current song
        /// </summary>
        BanCurrentSong,
        /// <summary>
        /// The pending songs play list is empty
        /// </summary>
        PlayListEmpty,
        /// <summary>
        /// Current song ended
        /// </summary>
        CurrentSongEnded,
        /// <summary>
        /// Like current song (mark red heart)
        /// </summary>
        Like,
        /// <summary>
        /// Cancel "like current song" (remove red heart mark)
        /// </summary>
        CancelLike,
    }
}
