using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    partial class Player
    {
        protected virtual Uri CreateGetChannelListUri()
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/app_channels");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.IconCategory, "xlarge");
            return uriBuilder.Uri;
        }

        protected virtual Uri CreateGetPlayListUri(int channelId, ReportType type, string sid, string formats, int? kbps, double? playedTime)
        {
            var uriBuilder = new UriBuilder("https://api.douban.com/v2/fm/playlist");
            uriBuilder.AppendUsageCommonFields(ServerConnection);
            uriBuilder.AppendQuery(StringTable.Channel, channelId.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.Type, ReportTypeString.GetString(type));
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            uriBuilder.AppendQuery(StringTable.Sid, sid);
            uriBuilder.AppendQuery(StringTable.Formats, formats);
            uriBuilder.AppendQuery(StringTable.Kbps, kbps?.ToString(CultureInfo.InvariantCulture));
            uriBuilder.AppendQuery(StringTable.PlayedTime, playedTime?.ToString("F1", CultureInfo.InvariantCulture));
            return uriBuilder.Uri;
        }

        protected virtual async Task<ChannelList> GetChannelList()
        {
            var jsonContent = await ServerConnection.Get(CreateGetChannelListUri(), RequestModifier);
            var newChannelList = ParseChannelList(jsonContent);
            var groupCount = newChannelList.ChannelGroups.Length;
            var channelCount = newChannelList.ChannelGroups.Sum(group => group.Channels.Length);
            Logger.Info($"Got channel list. Group count: {groupCount}. Channel count: {channelCount}. Detail: {JsonConvert.SerializeObject(newChannelList)}");
            return newChannelList;
        }

        protected virtual async Task<Song[]> GetPlayList(ReportType type)
        {
            object formats;
            Config.TryGetValue(StringTable.Formats, out formats);
            object kbps;
            Config.TryGetValue(StringTable.Formats, out kbps);

            var uri = CreateGetPlayListUri(CurrentChannel.Id, type, CurrentSong?.Sid, (string)formats, (int?)kbps, null /* TODO: fill played time here */);
            var jsonContent = await ServerConnection.Get(uri, RequestModifier);
            var newPlayList = ParsePlayList(jsonContent);
            Logger.Info($"Got play list. song count: {newPlayList.Length}. Detail: {JsonConvert.SerializeObject(newPlayList)}");
            return newPlayList;
        }

        protected virtual void RequestModifier(HttpWebRequest request)
        {
            var uri = request.RequestUri;
            if (!string.IsNullOrEmpty(ServerConnection.AccessToken) && uri.Host.Equals("api.douban.com", StringComparison.OrdinalIgnoreCase))
            {
                request.Headers["Authorization"] = "Bearer " + ServerConnection.AccessToken;
            }
        }
    }
}
