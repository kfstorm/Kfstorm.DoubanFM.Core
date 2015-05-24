using System;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kfstorm.DoubanFM.Core
{
    partial class Player
    {
        /// <summary>
        /// Creates the get play list URI.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="type">The report type.</param>
        /// <param name="sid">The SID of current song.</param>
        /// <param name="start">The start song code.</param>
        /// <param name="formats">The format of music file.</param>
        /// <param name="kbps">The bit rate of music file.</param>
        /// <param name="playedTime">The played time of current song.</param>
        /// <returns></returns>
        protected virtual Uri CreateGetPlayListUri(int channelId, ReportType type, string sid, string start, string formats, int? kbps, double? playedTime)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/playlist");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.Channel, channelId.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.Type, ReportTypeString.GetString(type));
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            uriBuilder.AppendQuery(StringTable.Start, start); // If start song code is not empty, then the first song returned will always be the same one.
            uriBuilder.AppendQuery(StringTable.Formats, formats);
            uriBuilder.AppendQuery(StringTable.Kbps, kbps?.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.PlayedTime, playedTime?.ToString("F1", CultureInfo.InvariantCulture));
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Gets the play list.
        /// </summary>
        /// <param name="type">The report type.</param>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="sid">The SID of current song.</param>
        /// <param name="start">The start song code.</param>
        /// <returns></returns>
        protected virtual async Task<Song[]> GetPlayList(ReportType type, int channelId, string sid, string start)
        {
            object formats;
            Config.TryGetValue(StringTable.Formats, out formats);
            object kbps;
            Config.TryGetValue(StringTable.Formats, out kbps);

            var uri = CreateGetPlayListUri(channelId, type, sid, start, (string)formats, (int?)kbps, null /* TODO: fill played time here */);
            var jsonContent = await ServerConnection.Get(uri, ServerConnection.SetSessionInfoToRequest);
            var newPlayList = ParsePlayList(jsonContent);
            Logger.Info($"Got play list. Type: {type}. Channel ID: {channelId}. Sid: {sid}. song count: {newPlayList.Length}. Detail: {JsonConvert.SerializeObject(newPlayList)}");
            return newPlayList;
        }
    }
}
