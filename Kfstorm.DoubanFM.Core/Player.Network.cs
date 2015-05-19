using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kfstorm.DoubanFM.Core
{
    partial class Player
    {
        /// <summary>
        /// Creates the get channel list URI.
        /// </summary>
        /// <returns></returns>
        protected virtual Uri CreateGetChannelListUri()
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/app_channels");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            // ReSharper disable once StringLiteralTypo
            uriBuilder.AppendQuery(StringTable.IconCategory, "xlarge");
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Creates the get play list URI.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="type">The report type.</param>
        /// <param name="sid">The SID of current song.</param>
        /// <param name="formats">The format of music file.</param>
        /// <param name="kbps">The bit rate of music file.</param>
        /// <param name="playedTime">The played time of current song.</param>
        /// <returns></returns>
        protected virtual Uri CreateGetPlayListUri(int channelId, ReportType type, string sid, string formats, int? kbps, double? playedTime)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/playlist");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.Channel, channelId.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.Type, ReportTypeString.GetString(type));
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            uriBuilder.AppendQuery(StringTable.Formats, formats);
            uriBuilder.AppendQuery(StringTable.Kbps, kbps?.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.PlayedTime, playedTime?.ToString("F1", CultureInfo.InvariantCulture));
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Gets the channel list.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<ChannelList> GetChannelList()
        {
            var jsonContent = await ServerConnection.Get(CreateGetChannelListUri(), ModifyRequest);
            var newChannelList = ParseChannelList(jsonContent);
            var groupCount = newChannelList.ChannelGroups.Length;
            var channelCount = newChannelList.ChannelGroups.Sum(group => group.Channels.Length);
            Logger.Info($"Got channel list. Group count: {groupCount}. Channel count: {channelCount}. Detail: {JsonConvert.SerializeObject(newChannelList)}");
            return newChannelList;
        }

        /// <summary>
        /// Gets the play list.
        /// </summary>
        /// <param name="type">The report type.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="sid">The SID of current song.</param>
        /// <returns></returns>
        protected virtual async Task<Song[]> GetPlayList(ReportType type, int channelId, string sid)
        {
            object formats;
            Config.TryGetValue(StringTable.Formats, out formats);
            object kbps;
            Config.TryGetValue(StringTable.Formats, out kbps);

            var uri = CreateGetPlayListUri(channelId, type, sid, (string)formats, (int?)kbps, null /* TODO: fill played time here */);
            var jsonContent = await ServerConnection.Get(uri, ModifyRequest);
            var newPlayList = ParsePlayList(jsonContent);
            Logger.Info($"Got play list. Type: {type}. Channel ID: {channelId}. Sid: {sid}. song count: {newPlayList.Length}. Detail: {JsonConvert.SerializeObject(newPlayList)}");
            return newPlayList;
        }

        /// <summary>
        /// Modifies the request.
        /// </summary>
        /// <param name="request">The request.</param>
        protected virtual void ModifyRequest(HttpWebRequest request)
        {
            var uri = request.RequestUri;
            if (!string.IsNullOrEmpty(ServerConnection.AccessToken) && uri.Host.Equals("api.douban.com", StringComparison.OrdinalIgnoreCase))
            {
                request.Headers["Authorization"] = "Bearer " + ServerConnection.AccessToken;
            }
        }
    }
}
