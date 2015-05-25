using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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
        /// Creates the get channel list URI.
        /// </summary>
        /// <returns></returns>
        protected virtual Uri CreateGetRecommendedChannelsUri()
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/app_channels");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            // ReSharper disable once StringLiteralTypo
            uriBuilder.AppendQuery(StringTable.IconCategory, "xlarge");
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Gets the recommended channels.
        /// </summary>
        /// <returns>The recommended channels, organized by groups.</returns>
        public async Task<ChannelGroup[]> GetRecommendedChannels()
        {
            var jsonContent = await ServerConnection.Get(CreateGetRecommendedChannelsUri(), ServerConnection.SetSessionInfoToRequest);
            return ParseRecommendedChannels(jsonContent);
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
        public async Task<PartialList<Channel>> SearchChannel(string query, int start, int size)
        {
            var uri = CreateSearchChannelUri(query, start, size);
            var jsonContent = await ServerConnection.Get(uri, null);
            return ParseSearchChannelResult(jsonContent);
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
        /// Gets the song detail.
        /// </summary>
        /// <param name="song">The song.</param>
        /// <returns></returns>
        public Task<SongDetail> GetSongDetail(Song song)
        {
            return GetSongDetail(song.Sid);
        }

        /// <summary>
        /// Creates the get channel info URI.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <returns></returns>
        protected virtual Uri CreateGetChannelInfoUri(int channelId)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/channel_info");
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
        /// Creates the get lyrics URI.
        /// </summary>
        /// <param name="sid">The SID of the song.</param>
        /// <param name="ssid">The SSID of the song.</param>
        /// <returns></returns>
        protected virtual Uri CreateGetLyricsUri(string sid, string ssid)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/lyric");
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            uriBuilder.AppendQuery(StringTable.Ssid, ssid);
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Gets the lyrics.
        /// </summary>
        /// <param name="sid">The SID of the song.</param>
        /// <param name="ssid">The SSID of the song.</param>
        /// <returns></returns>
        public async Task<string> GetLyrics(string sid, string ssid)
        {
            var uri = CreateGetLyricsUri(sid, ssid);
            var jsonContent = await ServerConnection.Get(uri, null);
            return ParseLyrics(jsonContent);
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
        /// Parses the recommended channels.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The recommended channels.</returns>
        protected virtual ChannelGroup[] ParseRecommendedChannels(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return (from @group in obj["groups"]
                    select new ChannelGroup
                    {
                        GroupId = (int)@group["group_id"],
                        GroupName = (string)@group["group_name"],
                        Channels = @group["chls"].GetArrayOrEmpty().Select(chl => chl.ParseChannel()).ToArray(),
                    }).ToArray();
        }

        /// <summary>
        /// Parses the search channel result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The search channel result.</returns>
        protected virtual PartialList<Channel> ParseSearchChannelResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            var channels = obj["channels"].GetArrayOrEmpty().Select(chl => chl.ParseChannel()).ToArray();
            var total = (int)obj["total"];
            return new PartialList<Channel>(channels, total);
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

        /// <summary>
        /// Parses the lyrics.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The lyrics if found, otherwise null.</returns>
        protected virtual string ParseLyrics(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            var lyrics = (string)obj["lyric"];
            if (lyrics == string.Empty) lyrics = null;
            return lyrics;
        }
    }
}
