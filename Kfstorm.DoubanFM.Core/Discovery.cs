using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    /// <summary>
    /// The default implementation of <see cref="IDiscovery"/>
    /// </summary>
    public class Discovery : IDiscovery
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected ILog Logger = LogManager.GetLogger(typeof(Player));

        /// <summary>
        /// Gets the server connection.
        /// </summary>
        /// <value>
        /// The server connection.
        /// </value>
        public IServerConnection ServerConnection { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Discovery" /> class.
        /// </summary>
        /// <param name="serverConnection">The server connection.</param>
        public Discovery(IServerConnection serverConnection)
        {
            ServerConnection = serverConnection;
        }

        /// <summary>
        /// Creates the search channel URI.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="start">The preferred index of the first channel in the returned channel array.</param>
        /// <param name="size">The max size of returned channel array.</param>
        /// <returns></returns>
        protected virtual Uri CreateSearchChannelUri(string query, int start, int size)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/search/channel");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.Query, query);
            uriBuilder.AppendQuery(StringTable.Start, start.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.Limit, size.ToString(CultureInfo.InvariantCulture));
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Searches the channel with specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="start">The preferred index of the first channel in the returned channel array.</param>
        /// <param name="size">The max size of returned channel array.</param>
        /// <returns>A channel array with the first channel at index <paramref name="start"/>, or an empty array if no channels available.</returns>
        public async Task<Channel[]> SearchChannel(string query, int start, int size)
        {
            var uri = CreateSearchChannelUri(query, start, size);
            var jsonContent = await ServerConnection.Get(uri, null);
            var channelArray = ParseSearchChannelResult(jsonContent);
            Logger.Info($"Got channel search result. Channel count: {channelArray.Length}. Detail: {JsonConvert.SerializeObject(channelArray)}");
            return channelArray;
        }

        /// <summary>
        /// Creates the get song detail URI.
        /// </summary>
        /// <param name="sid">The SID of the song.</param>
        /// <returns></returns>
        protected virtual Uri CreateGetSongDetailUri(string sid)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/song_detail");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Gets the song detail.
        /// </summary>
        /// <param name="sid">The SID of the song.</param>
        /// <returns></returns>
        public async Task<SongDetail> GetSongDetail(string sid)
        {
            var uri = CreateGetSongDetailUri(sid);
            var jsonContent = await ServerConnection.Get(uri, null);
            return ParseSongDetail(jsonContent);
        }

        /// <summary>
        /// Creates the get channel info URI.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <returns></returns>
        protected virtual Uri CreateGetChannelInfoUri(int channelId)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm//channel_info");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.Id, channelId.ToString(CultureInfo.InvariantCulture));
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Gets the channel info.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <returns></returns>
        public async Task<Channel> GetChannelInfo(int channelId)
        {
            var uri = CreateGetChannelInfoUri(channelId);
            var jsonContent = await ServerConnection.Get(uri, null);
            return ParseChannelInfo(jsonContent);
        }

        /// <summary>
        /// Parses the search channel result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The search channel result.</returns>
        protected virtual Channel[] ParseSearchChannelResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return obj["channels"].GetArrayOrEmpty().Select(chl => chl.ParseChannel()).ToArray();
        }

        /// <summary>
        /// Parses the song detail.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The song detail.</returns>
        protected virtual SongDetail ParseSongDetail(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return new SongDetail
            {
                ArtistChannels = obj["artist_channels"].GetArrayOrEmpty().Select(chl => chl.ParseChannel()).ToArray(),
                SimilarSongChannel = obj["similar_song_channel"].ParseOptional<int?>(),
            };
        }

        /// <summary>
        /// Parses the channel info.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The channel info.</returns>
        protected virtual Channel ParseChannelInfo(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            var channels = obj["data"]?["channels"].GetArrayOrEmpty();
            return channels?.Select(chl => chl.ParseChannel()).FirstOrDefault();
        }
    }
}
