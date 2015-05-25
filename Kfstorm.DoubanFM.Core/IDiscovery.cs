using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// Contains all the functionalities to discover songs.
    /// </summary>
    public interface IDiscovery
    {
        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        ISession Session { get; }

        /// <summary>
        /// Gets the server connection.
        /// </summary>
        /// <value>
        /// The server connection.
        /// </value>
        IServerConnection ServerConnection { get; }

        /// <summary>
        /// Gets the recommended channels.
        /// </summary>
        /// <returns>The recommended channels, organized by groups.</returns>
        Task<ChannelGroup[]> GetRecommendedChannels();

        /// <summary>
        /// Searches the channel with specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="start">The preferred index of the first channel in the returned channel array.</param>
        /// <param name="maxSize">The maximum size of returned channel array.</param>
        /// <returns>A channel array with the first channel at index <paramref name="start"/>, or an empty array if no channels available.</returns>
        Task<PartialList<Channel>> SearchChannel(string query, int start, int maxSize);

        /// <summary>
        /// Gets the song detail.
        /// </summary>
        /// <param name="sid">The SID of the song.</param>
        /// <returns></returns>
        Task<SongDetail> GetSongDetail(string sid);
        /// <summary>
        /// Gets the song detail.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <returns></returns>
        Task<SongDetail> GetSongDetail(Song song);
        /// <summary>
        /// Gets the channel info.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <returns></returns>
        Task<Channel> GetChannelInfo(int channelId);

        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        /// <param name="sid">The SID of the song.</param>
        /// <param name="ssid">The SSID of the song.</param>
        /// <returns>The lyrics if found, otherwise null.</returns>
        Task<string> GetLyrics(string sid, string ssid);
        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <returns>
        /// The lyrics if found, otherwise null.
        /// </returns>
        Task<string> GetLyrics(Song song);
        /// <summary>
        /// Gets the offline red heart songs.
        /// </summary>
        /// <param name="maxSize">The maximum amount of returned songs.</param>
        /// <param name="excludedSids">The excluded SIDs of songs.</param>
        /// <returns>
        /// The offline red heart songs.
        /// </returns>
        Task<Song[]> GetOfflineRedHeartSongs(int maxSize, IEnumerable<string> excludedSids);
        
        /// <summary>
        /// Updates the audio URL of the song.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <returns></returns>
        /// <remarks>The audio URL can be invalid after a period of time. This method can get an updated URL.</remarks>
        Task UpdateSongUrl(Song song);
    }
}
