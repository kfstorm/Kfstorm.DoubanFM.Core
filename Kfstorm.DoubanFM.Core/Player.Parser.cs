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
                        Channels = ((JArray)@group["chls"]).Select(chl => new Channel((int)chl["id"])
                        {
                            Name = (string)chl["name"],
                            Description = (string)chl["intro"],
                            SongCount = (int)chl["song_num"],
                            CoverUrl = (string)chl["cover"],
                        }).ToArray(),
                    }).ToArray()
            };
        }

        protected virtual Song[] ParsePlayList(string jsonContent)
        {
            var obj = JObject.Parse(jsonContent);
            return (from song in obj["song"]
                select new Song((string)song["sid"])
                {
                    AlbumUrl = (string)song["album"],
                    PictureUrl = (string)song["picture"],
                    Ssid = (string)song["ssid"],
                    Artist = (string)song["artist"],
                    Url = (string)song["url"],
                    Company = (string)song["company"],
                    Title = (string)song["title"],
                    AverageRating = (double)song["rating_avg"],
                    Length = (int)song["length"],
                    SubType = (string)song["subtype"],
                    PublishTime = (int)song["public_time"],
                    SongListsCount = (int)song["songlists_count"],
                    Aid = (string)song["aid"],
                    Sha256 = (string)song["sha256"],
                    Kbps = (int)song["kbps"],
                    AlbumTitle = (string)song["albumtitle"],
                    Like = (bool)song["like"],
                }).ToArray();
        }
    }
}
