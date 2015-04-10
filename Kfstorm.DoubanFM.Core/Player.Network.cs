using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    partial class Player
    {
        internal static string ChannelListUrlPattern = @"https://api.douban.com/v2/fm/app_channels?alt={json}&apikey={apikey}&app_name={appname}&client={clientinfo}&udid={udid}&version={appversion}";
        internal static string PlayListUrlPattern = @"https://api.douban.com/v2/fm/playlist?alt={json}&apikey={apikey}&app_name={appname}&client={clientinfo}&udid={udid}&version={appversion}&channel={channelid}&formats={format}&kbps={kbps}&pt={pasttime}&type={type}";

        protected virtual async Task<ChannelList> GetChannelList()
        {
            var jsonContent = await _serverConnection.Get(ChannelListUrlPattern);
            return ParseChannelList(jsonContent);
        }

        protected virtual async Task<Song[]> GetPlayList(GetPlayListType type)
        {
            var url = PlayListUrlPattern.Replace("{type}", GetPlayListTypeString.GetString(type));
            var jsonContent = await _serverConnection.Get(url);
            return ParsePlayList(jsonContent);
        }
    }
}
