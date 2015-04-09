using System.Threading.Tasks;

namespace Kfstorm.DoubanFM.Core
{
    partial class Player
    {
        internal static string ChannelListUrlPattern = @"https://api.douban.com/v2/fm/app_channels?alt={json}&apikey={apikey}&app_name={appname}&client={clientinfo}&udid={udid}&version={appversion}";
        protected virtual async Task<ChannelList> GetChannelList()
        {
            var jsonContent = await _serverConnection.Get(ChannelListUrlPattern);
            return ParseChannelList(jsonContent);
        }
    }
}
