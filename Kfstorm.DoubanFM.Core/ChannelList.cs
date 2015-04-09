using System.Linq;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    public class ChannelList
    {
        public ChannelGroup[] ChannelGroups { get; internal set; }

        internal ChannelList()
        {
            ChannelGroups = new ChannelGroup[0];
        }

        internal ChannelList(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            ChannelGroups = (from @group in obj["groups"]
                select new ChannelGroup
                {
                    GroupId = (int)@group["group_id"], GroupName = (string)@group["group_name"], Channels = ((JArray)@group["chls"]).Select(chl => new Channel
                    {
                        Name = (string)chl["name"], Id = (int)chl["id"], Description = (string)chl["intro"], SongCount = (int)chl["song_num"], CoverUrl = (string)chl["cover"],
                    }).ToArray(),
                }).ToArray();
        }
    }
}