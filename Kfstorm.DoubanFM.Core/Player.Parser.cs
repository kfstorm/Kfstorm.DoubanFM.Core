using System.Linq;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    partial class Player
    {
        protected virtual ChannelList ParseChannelList(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return new ChannelList
            {
                ChannelGroups = (from @group in obj["groups"]
                    select new ChannelGroup
                    {
                        GroupId = (int)@group["group_id"],
                        GroupName = (string)@group["group_name"],
                        Channels = ((JArray)@group["chls"]).Select(chl => new Channel
                        {
                            Name = (string)chl["name"],
                            Id = (int)chl["id"],
                            Description = (string)chl["intro"],
                            SongCount = (int)chl["song_num"],
                            CoverUrl = (string)chl["cover"],
                        }).ToArray(),
                    }).ToArray()
            };
        }
    }
}
