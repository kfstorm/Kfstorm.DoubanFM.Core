using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// The default implementation of <see cref="IDiscovery"/>
    /// </summary>
    public class Discovery : IDiscovery
    {
        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public ISession Session { get; }

        /// <summary>
        /// Gets the server connection.
        /// </summary>
        /// <value>
        /// The server connection.
        /// </value>
        public IServerConnection ServerConnection => Session.ServerConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Discovery" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public Discovery(ISession session)
        {
            Session = session;
        }

        /// <summary>
        /// Gets the recommended channels.
        /// </summary>
        /// <returns>The recommended channels, organized by groups.</returns>
        public async Task<ChannelGroup[]> GetRecommendedChannels()
        {
            var uri = ServerConnection.CreateGetRecommendedChannelsUri();
            var jsonContent = await ServerConnection.Get(uri, ServerConnection.SetSessionInfoToRequest);
            return ServerRequests.ParseGetRecommendedChannelsResult(jsonContent);
        }

        /// <summary>
        /// Searches the channel with specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="start">The preferred index of the first channel in the returned channel array.</param>
        /// <param name="maxSize">The maximum size of returned channel array.</param>
        /// <returns>A channel array with the first channel at index <paramref name="start"/>, or an empty array if no channels available.</returns>
        public async Task<PartialList<Channel>> SearchChannel(string query, int start, int maxSize)
        {
            var uri = ServerConnection.CreateSearchChannelUri(query, start, maxSize);
            var jsonContent = await ServerConnection.Get(uri, null);
            return ServerRequests.ParseSearchChannelResult(jsonContent);
        }

        /// <summary>
        /// Gets the song detail.
        /// </summary>
        /// <param name="sid">The SID of the song.</param>
        /// <returns></returns>
        public async Task<SongDetail> GetSongDetail(string sid)
        {
            var uri = ServerConnection.CreateGetSongDetailUri(sid);
            var jsonContent = await ServerConnection.Get(uri, null);
            return ServerRequests.ParseGetSongDetailResult(jsonContent);
        }

        /// <summary>
        /// Gets the song detail.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <returns></returns>
        public Task<SongDetail> GetSongDetail(Song song)
        {
            return GetSongDetail(song.Sid);
        }

        /// <summary>
        /// Gets the channel info.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <returns></returns>
        public async Task<Channel> GetChannelInfo(int channelId)
        {
            var uri = ServerConnection.CreateGetChannelInfoUri(channelId);
            var jsonContent = await ServerConnection.Get(uri, null);
            return ServerRequests.ParseGetChannelInfoResult(jsonContent);
        }

        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        /// <param name="sid">The SID of the song.</param>
        /// <param name="ssid">The SSID of the song.</param>
        /// <returns></returns>
        public async Task<string> GetLyrics(string sid, string ssid)
        {
            var uri = ServerRequests.CreateGetLyricsUri(sid, ssid);
            var jsonContent = await ServerConnection.Get(uri, null);
            return ServerRequests.ParseGetLyricsResult(jsonContent);
        }

        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <returns>
        /// The lyrics if found, otherwise null.
        /// </returns>
        public Task<string> GetLyrics(Song song)
        {
            return GetLyrics(song.Sid, song.Ssid);
        }

        /// <summary>
        /// Gets the offline red heart songs.
        /// </summary>
        /// <param name="maxSize">The maximum amount of returned songs.</param>
        /// <param name="excludedSids">The excluded SIDs of songs.</param>
        /// <returns>
        /// The offline red heart songs.
        /// </returns>
        public async Task<Song[]> GetOfflineRedHeartSongs(int maxSize, IEnumerable<string> excludedSids)
        {
            var uri = ServerConnection.CreateGetPlayListUri(-3, type: ReportType.CurrentChannelChanged, sid: null, start: null, formats: null, kbps: null, playedTime: null, mode: "offline", excludedSids: excludedSids, max: maxSize);
            var jsonContent = await ServerConnection.Get(uri, ServerConnection.SetSessionInfoToRequest);
            return ServerRequests.ParseGetPlayListResult(jsonContent);
        }

        /// <summary>
        /// Updates the audio URL of the song.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <returns></returns>
        /// <remarks>The audio URL can be invalid after a period of time. This method can get an updated URL.</remarks>
        public async Task UpdateSongUrl(Song song)
        {
            var uri = ServerConnection.CreateGetSongUrlUri(song.Sid, song.Ssid);
            var jsonContent = await ServerConnection.Get(uri, ServerConnection.SetSessionInfoToRequest);
            song.Url = ServerRequests.ParseGetSongUrlResult(jsonContent);
        }
    }
}
