using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    internal static class ServerRequests
    {
        #region playlist

        /// <summary>
        /// Creates the get play list URI.
        /// </summary>
        /// <param name="connection">The server connection.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="type">The report type.</param>
        /// <param name="sid">The SID of current song.</param>
        /// <param name="start">The start song code.</param>
        /// <param name="formats">The format of music file.</param>
        /// <param name="kbps">The bit rate of music file.</param>
        /// <param name="playedTime">The played time of current song.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="excludedSids">The excluded SIDs.</param>
        /// <param name="max">The maximum size of returned play list.</param>
        /// <returns></returns>
        public static Uri CreateGetPlayListUri(this IServerConnection connection, int channelId, ReportType type, string sid, string start, string formats, int? kbps, double? playedTime, string mode, IEnumerable<string> excludedSids, int? max)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/playlist");
            uriBuilder.AppendUsageCommonFields(connection);
            uriBuilder.AppendQuery(StringTable.Channel, channelId.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.Type, ReportTypeString.GetString(type));
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            uriBuilder.AppendQuery(StringTable.Start, start); // If start song code is not empty, then the first song returned will always be the same one.
            uriBuilder.AppendQuery(StringTable.Formats, formats);
            uriBuilder.AppendQuery(StringTable.Kbps, kbps?.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.PlayedTime, playedTime?.ToString("F1", CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.Mode, mode);
            uriBuilder.AppendQuery(StringTable.Exclude, excludedSids == null ? null : String.Join(",", excludedSids));
            uriBuilder.AppendQuery(StringTable.Max, max?.ToString(CultureInfo.InvariantCulture));
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Parses the get play list result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        public static Song[] ParseGetPlayListResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            JToken songs;
            if (obj.TryGetValue("song", out songs) && songs != null)
            {
                return (from song in songs
                        select song.ParseSong()).ToArray();
            }
            return new Song[0];
        }

        #endregion

        #region app_channels

        /// <summary>
        /// Creates the get recommended channels URI.
        /// </summary>
        /// <param name="connection">The server connection.</param>
        /// <returns></returns>
        public static Uri CreateGetRecommendedChannelsUri(this IServerConnection connection)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/app_channels");
            uriBuilder.AppendUsageCommonFields(connection);
            // ReSharper disable once StringLiteralTypo
            uriBuilder.AppendQuery(StringTable.IconCategory, "xlarge");
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Parses the get recommended channels result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns></returns>
        public static ChannelGroup[] ParseGetRecommendedChannelsResult(string jsonContent)
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

        #endregion

        #region search/channel

        /// <summary>
        /// Creates the search channel URI.
        /// </summary>
        /// <param name="connection">The server connection.</param>
        /// <param name="query">The query.</param>
        /// <param name="start">The preferred index of the first channel in the returned channel array.</param>
        /// <param name="size">The max size of returned channel array.</param>
        /// <returns></returns>
        public static Uri CreateSearchChannelUri(this IServerConnection connection, string query, int start, int size)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/search/channel");
            uriBuilder.AppendUsageCommonFields(connection);
            uriBuilder.AppendQuery(StringTable.Query, query);
            uriBuilder.AppendQuery(StringTable.Start, start.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.Limit, size.ToString(CultureInfo.InvariantCulture));
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Parses the search channel result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        public static PartialList<Channel> ParseSearchChannelResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            var channels = obj["channels"].GetArrayOrEmpty().Select(chl => chl.ParseChannel()).ToArray();
            var total = (int)obj["total"];
            return new PartialList<Channel>(channels, total);
        }

        #endregion

        #region song_detail

        /// <summary>
        /// Creates the get song detail URI.
        /// </summary>
        /// <param name="connection">The server connection.</param>
        /// <param name="sid">The SID of the song.</param>
        /// <returns></returns>
        public static Uri CreateGetSongDetailUri(this IServerConnection connection, string sid)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/song_detail");
            uriBuilder.AppendUsageCommonFields(connection);
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Parses the get song detail result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns></returns>
        public static SongDetail ParseGetSongDetailResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return new SongDetail
            {
                ArtistChannels = obj["artist_channels"].GetArrayOrEmpty().Select(chl => chl.ParseChannel()).ToArray(),
                SimilarSongChannel = obj["similar_song_channel"].ParseOptional<int?>(),
            };
        }

        #endregion

        #region channel_info

        /// <summary>
        /// Creates the get channel info URI.
        /// </summary>
        /// <param name="connection">The server connection.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <returns></returns>
        public static Uri CreateGetChannelInfoUri(this IServerConnection connection, int channelId)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/channel_info");
            uriBuilder.AppendUsageCommonFields(connection);
            uriBuilder.AppendQuery(StringTable.Id, channelId.ToString(CultureInfo.InvariantCulture));
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Parses the get channel information result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns></returns>
        public static Channel ParseGetChannelInfoResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            var channels = obj["data"]?["channels"].GetArrayOrEmpty();
            return channels?.Select(chl => chl.ParseChannel()).FirstOrDefault();
        }

        #endregion

        #region lyric

        /// <summary>
        /// Creates the get lyrics URI.
        /// </summary>
        /// <param name="sid">The SID of the song.</param>
        /// <param name="ssid">The SSID of the song.</param>
        /// <returns></returns>
        public static Uri CreateGetLyricsUri(string sid, string ssid)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/lyric");
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            uriBuilder.AppendQuery(StringTable.Ssid, ssid);
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Parses the get lyrics result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The lyrics if found, otherwise null.</returns>
        public static string ParseGetLyricsResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            var lyrics = (string)obj["lyric"];
            if (lyrics == string.Empty) lyrics = null;
            return lyrics;
        }

        #endregion

        #region song_url

        /// <summary>
        /// Creates the get song URL URI.
        /// </summary>
        /// <param name="connection">The server connection.</param>
        /// <param name="sid">The SID of the song.</param>
        /// <param name="ssid">The SSID of the song.</param>
        /// <returns></returns>
        public static Uri CreateGetSongUrlUri(this IServerConnection connection, string sid, string ssid)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/song_url");
            uriBuilder.AppendUsageCommonFields(connection);
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            uriBuilder.AppendQuery(StringTable.Ssid, ssid);
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Parses the get song URL result.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns></returns>
        public static string ParseGetSongUrlResult(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return (string)obj["url"];
        }

        #endregion
    }
}
