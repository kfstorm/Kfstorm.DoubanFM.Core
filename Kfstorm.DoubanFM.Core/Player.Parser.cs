using System.Linq;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    partial class Player
    {
        /// <summary>
        /// Parses the channel list.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The channel list.</returns>
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
                                     Channels = @group["chls"].GetArrayOrEmpty().Select(chl => chl.ParseChannel()).ToArray(),
                                 }).ToArray()
            };
        }

        /// <summary>
        /// Parses the play list.
        /// </summary>
        /// <param name="jsonContent">Content of JSON format.</param>
        /// <returns>The play list.</returns>
        protected virtual Song[] ParsePlayList(string jsonContent)
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
    }
}
