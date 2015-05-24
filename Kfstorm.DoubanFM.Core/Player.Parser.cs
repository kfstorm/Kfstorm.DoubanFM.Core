using System.Linq;
using Newtonsoft.Json.Linq;

namespace Kfstorm.DoubanFM.Core
{
    partial class Player
    {
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
