namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Contains information to discover relative songs
    /// </summary>
    public class SongDetail
    {
        /// <summary>
        /// Gets or sets the artist channels.
        /// </summary>
        /// <value>
        /// The artist channels.
        /// </value>
        public Channel[] ArtistChannels { get; set; }
        /// <summary>
        /// Gets or sets the channel ID of similar songs.
        /// </summary>
        /// <value>
        /// The channel ID of similar songs.
        /// </value>
        public int? SimilarSongChannel { get; set; }
        
        // TODO: Related programmes
    }
}
